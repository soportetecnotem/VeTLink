using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VeTLink.Data;
using VeTLink.DTOs.Persona;
using VeTLink.DTOs.Responses;
using VeTLink.DTOs.Usuario;
using VeTLink.Models;
using VeTLink.Services;
using VeTLink.Utilidades;

namespace VeTLink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [DeshabilitarLimitarPeticiones]
    public class AuthController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context,
        IConfiguration config,
        IMapper mapper,
        IServicioLlaves servicioLlaves) : ControllerBase
    {

        [HttpPost("Nuevo")]
        [EndpointSummary("Registrar nuevo usuario.")]
        public async Task<ActionResult<RespuestaGeneralDTO>> Register(RegisterDto model)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            // Validar que el email no esté registrado
            var emailExistente = await userManager.FindByEmailAsync(model.Email);
            if (emailExistente != null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Ya existe una cuenta registrada con este correo electrónico.");
                return BadRequest(respuesta);
            }

            var user = mapper.Map<IdentityUser>(model);
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var persona = mapper.Map<Persona>(model);
                persona.UsuarioId = user.Id;
                persona.TipoUsuarioId = 3;

                context.Personas.Add(persona);
                await context.SaveChangesAsync();

                // Crear rol AdminClinica si no existe
                if (!await roleManager.RoleExistsAsync("AdminClinica"))
                {
                    await roleManager.CreateAsync(new IdentityRole("AdminClinica"));
                }

                // Asignar rol AdminClinica al usuario
                await userManager.AddToRoleAsync(user, "AdminClinica");

                //Asignar Llave Gratuita al usuario
                await servicioLlaves.CrearLlave(user.Id, TipoLlave.Gratuita);

                respuesta.Status = true;
                respuesta.Message.Add("Usuario creado exitosamente con rol AdminClinica. Version Gratuita");
                return Ok(respuesta);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    var customMessage = MapIdentityError(error);
                    respuesta.Message.Add(customMessage);
                }

                return BadRequest(respuesta);
            }
        }

        [HttpGet("Detalles/{userName}")]
        [Authorize]
        [EndpointSummary("Obtiene los detalles de un usuario por su UserName.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetUsuarioPorUserName(string userName)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            var usuario = await userManager.FindByNameAsync(userName);

            if (usuario is null)
            {
                respuesta.Message.Add("Usuario no encontrado.");
                return NotFound(respuesta);
            }

            var usuarioDTO = mapper.Map<UsuarioDTO>(usuario);
            usuarioDTO.Roles = await (from ur in context.UserRoles
                                      join r in context.Roles on ur.RoleId equals r.Id
                                      where ur.UserId == usuario.Id
                                      select r.Name).ToListAsync();

            respuesta.Status = true;
            respuesta.Message.Add("Detalles del usuario recuperados.");
            respuesta.Response = usuarioDTO;

            return respuesta;
        }

        [HttpPut("Actualizar/{userId}")]
        [Authorize]
        [EndpointSummary("Actualiza los datos de un usuario incluyendo roles")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ActualizarUsuario(string userId, [FromBody] ActualizarUsuarioDTO model)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Buscar el usuario
                var usuario = await userManager.FindByIdAsync(userId);
                if (usuario == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Usuario no encontrado.");
                    return NotFound(respuesta);
                }

                // Buscar la persona asociada
                var persona = await context.Personas
                    .FirstOrDefaultAsync(p => p.UsuarioId == userId);

                if (persona == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("No se encontró el perfil de persona asociado.");
                    return NotFound(respuesta);
                }

                // Actualizar email si cambió y validar que no esté en uso
                if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != usuario.Email)
                {
                    var emailExistente = await userManager.FindByEmailAsync(model.Email);
                    if (emailExistente != null && emailExistente.Id != userId)
                    {
                        respuesta.Status = false;
                        respuesta.Message.Add("El correo electrónico ya está en uso por otro usuario.");
                        return BadRequest(respuesta);
                    }

                    usuario.Email = model.Email;
                    usuario.UserName = model.Email; // Mantener sincronizado
                    usuario.NormalizedEmail = model.Email.ToUpper();
                    usuario.NormalizedUserName = model.Email.ToUpper();
                }

                // Actualizar datos de la persona
                if (!string.IsNullOrWhiteSpace(model.Nombre))
                    persona.Nombre = model.Nombre;

                if (!string.IsNullOrWhiteSpace(model.PrimerApellido))
                    persona.PrimerApellido = model.PrimerApellido;

                persona.SegundoApellido = model.SegundoApellido ?? persona.SegundoApellido;
                persona.Genero = model.Genero ?? persona.Genero;
                persona.FechaNacimiento = model.FechaNacimiento ?? persona.FechaNacimiento;
                persona.NumeroIdentificacion = model.NumeroIdentificacion ?? persona.NumeroIdentificacion;
                persona.Telefono = model.Telefono ?? persona.Telefono;
                persona.Imagen = model.Imagen ?? persona.Imagen;

                // Recalcular edad si cambió la fecha de nacimiento
                if (persona.FechaNacimiento.HasValue && persona.FechaNacimiento.Value != default)
                {
                    var today = DateTime.Today;
                    var edad = today.Year - persona.FechaNacimiento.Value.Year;
                    if (persona.FechaNacimiento.Value.Date > today.AddYears(-edad))
                    {
                        edad--;
                    }
                    persona.Edad = edad >= 0 ? edad : null;
                }

                // Actualizar TipoUsuario si cambió
                if (model.TipoUsuarioId.HasValue && model.TipoUsuarioId.Value != persona.TipoUsuarioId)
                {
                    persona.TipoUsuarioId = model.TipoUsuarioId.Value;
                }

                // Actualizar el usuario en Identity
                var resultUsuario = await userManager.UpdateAsync(usuario);
                if (!resultUsuario.Succeeded)
                {
                    await transaction.RollbackAsync();
                    respuesta.Status = false;
                    foreach (var error in resultUsuario.Errors)
                    {
                        respuesta.Message.Add(MapIdentityError(error));
                    }
                    return BadRequest(respuesta);
                }

                // Actualizar persona en la base de datos
                context.Personas.Update(persona);
                await context.SaveChangesAsync();

                // ========================================
                // ACTUALIZAR ROLES
                // ========================================
                if (model.Roles != null && model.Roles.Any())
                {
                    // Obtener roles actuales
                    var rolesActuales = await userManager.GetRolesAsync(usuario);

                    // Roles a agregar (están en model.Roles pero no en rolesActuales)
                    var rolesParaAgregar = model.Roles.Except(rolesActuales, StringComparer.OrdinalIgnoreCase).ToList();

                    // Roles a eliminar (están en rolesActuales pero no en model.Roles)
                    var rolesParaEliminar = rolesActuales.Except(model.Roles, StringComparer.OrdinalIgnoreCase).ToList();

                    // Validar y crear roles que no existan
                    foreach (var rol in rolesParaAgregar)
                    {
                        if (!await roleManager.RoleExistsAsync(rol))
                        {
                            await roleManager.CreateAsync(new IdentityRole(rol));
                        }
                    }

                    // Eliminar roles
                    if (rolesParaEliminar.Any())
                    {
                        var resultRemove = await userManager.RemoveFromRolesAsync(usuario, rolesParaEliminar);
                        if (!resultRemove.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            respuesta.Status = false;
                            foreach (var error in resultRemove.Errors)
                            {
                                respuesta.Message.Add($"Error al eliminar rol: {error.Description}");
                            }
                            return BadRequest(respuesta);
                        }
                    }

                    // Agregar roles
                    if (rolesParaAgregar.Any())
                    {
                        var resultAdd = await userManager.AddToRolesAsync(usuario, rolesParaAgregar);
                        if (!resultAdd.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            respuesta.Status = false;
                            foreach (var error in resultAdd.Errors)
                            {
                                respuesta.Message.Add($"Error al agregar rol: {error.Description}");
                            }
                            return BadRequest(respuesta);
                        }
                    }
                }

                // ========================================
                // CAMBIO DE CONTRASEÑA (OPCIONAL)
                // ========================================
                if (!string.IsNullOrWhiteSpace(model.NuevaContrasena))
                {
                    // Si se proporciona contraseña actual, verificarla
                    if (!string.IsNullOrWhiteSpace(model.ContrasenaActual))
                    {
                        var resultPassword = await userManager.ChangePasswordAsync(usuario, model.ContrasenaActual, model.NuevaContrasena);
                        if (!resultPassword.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            respuesta.Status = false;
                            respuesta.Message.Add("La contraseña actual es incorrecta.");
                            foreach (var error in resultPassword.Errors)
                            {
                                respuesta.Message.Add(MapIdentityError(error));
                            }
                            return BadRequest(respuesta);
                        }
                    }
                    else
                    {
                        // Si no se proporciona contraseña actual, solo admin puede cambiarla
                        var esAdmin = User.IsInRole("Admin");
                        if (!esAdmin)
                        {
                            respuesta.Status = false;
                            respuesta.Message.Add("Debe proporcionar la contraseña actual para cambiarla.");
                            return BadRequest(respuesta);
                        }

                        // Admin cambia contraseña sin necesidad de la actual
                        var token = await userManager.GeneratePasswordResetTokenAsync(usuario);
                        var resultPassword = await userManager.ResetPasswordAsync(usuario, token, model.NuevaContrasena);

                        if (!resultPassword.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            respuesta.Status = false;
                            foreach (var error in resultPassword.Errors)
                            {
                                respuesta.Message.Add(MapIdentityError(error));
                            }
                            return BadRequest(respuesta);
                        }
                    }
                }

                await transaction.CommitAsync();

                // Obtener datos actualizados para la respuesta
                var personaActualizada = await context.Personas
                    .Include(p => p.TipoUsuario)
                    .Include(p => p.Direccion)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == persona.Id);

                var personaDto = mapper.Map<DetallePersonaDTO>(personaActualizada);

                // Agregar roles a la respuesta
                var rolesFinales = await userManager.GetRolesAsync(usuario);

                respuesta.Status = true;
                respuesta.Message.Add("Usuario actualizado exitosamente.");
                respuesta.Response = new
                {
                    Usuario = personaDto,
                    Roles = rolesFinales
                };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                respuesta.Status = false;
                respuesta.Message.Add($"Error al actualizar el usuario: {ex.Message}");
                return StatusCode(500, respuesta);
            }
        }

        [HttpGet("Listado")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Obtiene el listado completo de usuarios del sistema (solo Admin)")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ObtenerListadoUsuarios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? buscar = null,
            [FromQuery] int? tipoUsuarioId = null,
            [FromQuery] string? rol = null)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                // Query base con todas las relaciones
                var query = context.Personas
                    .Include(p => p.Usuario)
                    .Include(p => p.TipoUsuario)
                    .Include(p => p.Clinica)
                    .Include(p => p.Direccion)
                    .AsQueryable();

                // Filtro por búsqueda (nombre, apellido, email)
                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    var buscarLower = buscar.ToLower();
                    query = query.Where(p =>
                        p.Nombre.ToLower().Contains(buscarLower) ||
                        p.PrimerApellido.ToLower().Contains(buscarLower) ||
                        (p.SegundoApellido != null && p.SegundoApellido.ToLower().Contains(buscarLower)) ||
                        p.Usuario.Email!.ToLower().Contains(buscarLower));
                }

                // Filtro por tipo de usuario
                if (tipoUsuarioId.HasValue)
                {
                    query = query.Where(p => p.TipoUsuarioId == tipoUsuarioId.Value);
                }

                // Filtro por rol
                if (!string.IsNullOrWhiteSpace(rol))
                {
                    var usuariosConRol = await (from ur in context.UserRoles
                                                join r in context.Roles on ur.RoleId equals r.Id
                                                where r.Name == rol
                                                select ur.UserId).ToListAsync();

                    query = query.Where(p => usuariosConRol.Contains(p.UsuarioId));
                }

                // Total de registros
                var total = await query.CountAsync();

                // Paginación
                var personas = await query
                    .OrderBy(p => p.Nombre)
                    .ThenBy(p => p.PrimerApellido)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                // Mapear a DTO y agregar roles
                var usuariosDTO = new List<UsuarioSistemaDTO>();

                foreach (var persona in personas)
                {
                    var usuarioDto = new UsuarioSistemaDTO
                    {
                        Id = persona.Id,
                        UsuarioId = persona.UsuarioId,
                        Email = persona.Usuario.Email ?? "",
                        Nombre = persona.Nombre,
                        PrimerApellido = persona.PrimerApellido,
                        SegundoApellido = persona.SegundoApellido,
                        NombreCompleto = $"{persona.Nombre} {persona.PrimerApellido} {persona.SegundoApellido ?? ""}".Trim(),
                        Genero = persona.Genero,
                        FechaNacimiento = persona.FechaNacimiento,
                        Edad = persona.Edad,
                        NumeroIdentificacion = persona.NumeroIdentificacion,
                        Telefono = persona.Telefono,
                        Imagen = persona.Imagen,
                        TipoUsuarioId = persona.TipoUsuarioId,
                        TipoUsuarioNombre = persona.TipoUsuario?.Nombre,
                        ClinicaId = persona.ClinicaId,
                        NombreClinica = persona.Clinica?.NombreClinica,
                        EmailConfirmado = persona.Usuario.EmailConfirmed,
                        CuentaBloqueada = persona.Usuario.LockoutEnd.HasValue && persona.Usuario.LockoutEnd.Value > DateTimeOffset.UtcNow
                    };

                    // Obtener roles del usuario
                    var roles = await (from ur in context.UserRoles
                                       join r in context.Roles on ur.RoleId equals r.Id
                                       where ur.UserId == persona.UsuarioId
                                       select r.Name).ToListAsync();

                    usuarioDto.Roles = roles;

                    usuariosDTO.Add(usuarioDto);
                }

                respuesta.Status = true;
                respuesta.Message.Add($"Se encontraron {total} usuario(s).");
                respuesta.Response = new
                {
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize),
                    data = usuariosDTO
                };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener el listado de usuarios: {ex.Message}");
                return StatusCode(500, respuesta);
            }
        }

        [HttpPost("login")]
        [EndpointSummary("Obtiene token al ingresar las credenciales correctas")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Login(LoginDto model)
        {
            // "email": "priscilatafolla@gmail.com","password": "Abc123#." 

        var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                respuesta.Message.Add("El usuario no se encuentra.");
                return respuesta;
            }
            var resultado = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                //Obtener persona vinculada al usuario
                var persona = await context.Personas
                    .Include(p => p.TipoUsuario)
                    .FirstOrDefaultAsync(p => p.UsuarioId == user.Id);

                var personaDto = mapper.Map<DetallePersonaDTO>(persona);

                //var token = GenerateJwtTokenAsync(user);
                var token = ConstruirToken(user);
                
                respuesta.Status = true;
                respuesta.Message.Add("Login correcto.");
                respuesta.Response = new
                {
                    Token = token.Result.Token,
                    Usuario = personaDto
                };
                return respuesta;
            }
            else
            {
                respuesta.Message.Add("Revise el usuario o contraseña e intente de nuevo.");
                return Unauthorized(respuesta);
            }

        }

        [HttpPost("forgotpassword")]
        [EndpointSummary("Genera un token para restablecer la contraseña del usuario y lo envía por correo")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ForgotPassword(ForgotPasswordDto model, [FromServices] IEmailService emailService)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                respuesta.Message.Add("El correo no se encuentra registrado.");
                return NotFound(respuesta);
            }

            // Generar token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Codificar el token y email para URL
            var encodedToken = Uri.EscapeDataString(token);
            //var encodedEmail = Uri.EscapeDataString(model.Email);
            var encodedEmail = model.Email;

            // Construir link hacia el frontend con parámetros en query string
            var resetLink = $"https://vetlink.pages.dev/auth/resetpassword?token={encodedToken}&email={encodedEmail}";

            // Construir mensaje HTML
            var mensajeHtml = $@"
                <h2>Recuperación de contraseña</h2>
                <p>Hola, {user.UserName}:</p>
                <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                <p>Da clic en el siguiente enlace para continuar:</p>
                <p><a href='{resetLink}' target='_blank'>Restablecer contraseña</a></p>
                <p>Si no solicitaste este cambio, ignora este mensaje.</p>
                <hr>
                <p style='font-size:12px;color:#888;'>Este enlace es válido por tiempo limitado.</p>
            ";

            // Enviar correo
            await emailService.SendEmailAsync(model.Email, "Recupera tu contraseña", mensajeHtml);

            respuesta.Status = true;
            respuesta.Message.Add("Se ha enviado un correo con el enlace para restablecer tu contraseña.");
            return Ok(respuesta);
        }

        [HttpPost("resetpassword")]
        [EndpointSummary("Permite al usuario cambiar su contraseña usando un token válido")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ResetPassword(ResetPasswordDto model)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                respuesta.Message.Add("Correo no encontrado.");
                return NotFound(respuesta);
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                respuesta.Message.AddRange(result.Errors.Select(e => e.Description));
                return BadRequest(respuesta);
            }

            respuesta.Status = true;
            respuesta.Message.Add("La contraseña se ha restablecido correctamente.");
            return Ok(respuesta);
        }

        [HttpPost("PrimerUsuario")]
        public async Task<IActionResult> RegisterFirstUserAdmin(RegisterDto model)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            // Validar que el email no esté registrado
            var emailExistente = await userManager.FindByEmailAsync(model.Email);
            if (emailExistente != null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Ya existe una cuenta registrada con este correo electrónico.");
                return BadRequest(respuesta);
            }

            var user = mapper.Map<IdentityUser>(model);
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                respuesta.Status = false;
                foreach (var error in result.Errors)
                {
                    var customMessage = MapIdentityError(error);
                    respuesta.Message.Add(customMessage);
                }
                return BadRequest(respuesta);
            }

            // Mapear a Persona
            var persona = mapper.Map<Persona>(model);
            persona.UsuarioId = user.Id;

            context.Personas.Add(persona);
            await context.SaveChangesAsync();

            // Verificar si es el primer usuario
            var totalUsuarios = userManager.Users.Count();
            if (totalUsuarios == 1)
            {
                // Crear rol Admin si no existe
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Asignar rol Admin al primer usuario
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                // Asignar rol por defecto a los demás usuarios
                if (!await roleManager.RoleExistsAsync("AdminClinica"))
                    await roleManager.CreateAsync(new IdentityRole("AdminClinica"));

                await userManager.AddToRoleAsync(user, "AdminClinica");
            }

            var token = await ConstruirToken(user);

            respuesta.Status = true;
            respuesta.Message.Add("Usuario registrado exitosamente.");
            respuesta.Response = new { Token = token };

            return Ok(respuesta);
        }

        [HttpDelete("Eliminar/{username}")]
        [Authorize]
        [EndpointSummary("Elimina un usuario por su nombre de usuario (UserName)")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarUsuario(string username)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            var usuario = await userManager.FindByNameAsync(username);
            if (usuario == null)
            {
                respuesta.Message.Add("Usuario no encontrado.");
                return respuesta;
            }
            try
            { 
            var result = await userManager.DeleteAsync(usuario);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        respuesta.Message.Add(error.Description);
                    }
                    return respuesta;
                }
                respuesta.Status = true;
                respuesta.Message.Add("Usuario eliminado correctamente.");
                respuesta.Response = username;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El usuario tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }           
            return respuesta;
        }

        [HttpGet("renovar-token")]
        [EndpointSummary("Renueva token de un usuario")]
        [Authorize] // Solo usuarios con token válido pueden renovar
        public async Task<ActionResult<RespuestaObjetoDTO>> RenovarToken()
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            try
            {
                // Obtener el ID del usuario autenticado
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    respuesta.Status = false;
                    respuesta.Message.Add( "Usuario no válido." );
                    return Unauthorized(respuesta);
                }

                // Buscar al usuario en Identity
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add( "Usuario no encontrado." );
                    return NotFound(respuesta);
                }

                // Generar un nuevo JWT
                var token = ConstruirToken(user);

                // Retornar respuesta
                respuesta.Status = true;
                respuesta.Message.Add( "Token renovado correctamente." );
                respuesta.Response = new { Token = token };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Error al renovar token." + ex.Message );
                return StatusCode(500, respuesta);
            }
        }

        //private async Task<TokenDTO> GenerateJwtTokenAsync(IdentityUser user)
        //{
        //    var jwtSettings = config.GetSection("Jwt");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var claims = new List<Claim>
        //    {
        //        // IMPORTANTE: Usar ClaimTypes.NameIdentifier en lugar de "UserId"
        //        new Claim(ClaimTypes.NameIdentifier, user.Id),
        //        new Claim(ClaimTypes.Email, user.Email ?? ""),
        //        new Claim(ClaimTypes.Name, user.UserName ?? "")
        //    };

        //    var usuario = await userManager.FindByEmailAsync(user.Email!);
        //    var roles = await userManager.GetRolesAsync(usuario!);
        //    foreach (var rol in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, rol));
        //    }

        //    var expiracion = DateTime.UtcNow.AddDays(1);

        //    var TokenSeguridad = new JwtSecurityToken(issuer: null, audience: null,
        //                   claims: claims, expires: expiracion, signingCredentials: creds);

        //    var token = new JwtSecurityTokenHandler().WriteToken(TokenSeguridad);
        //    return new TokenDTO
        //    {
        //        Token = token,
        //        Expiracion = expiracion
        //    };
        //}

        private async Task<RespuestaAutenticacionDTO> ConstruirToken(
            IdentityUser credencialesUsuarioDTO)
        {
            var claims = new List<Claim>
            {
                // IMPORTANTE: Usar ClaimTypes.NameIdentifier en lugar de "UserId"
                new Claim(ClaimTypes.NameIdentifier, credencialesUsuarioDTO.Id),
                new Claim(ClaimTypes.Email, credencialesUsuarioDTO.Email!),
                new Claim("UserId", credencialesUsuarioDTO.Id!),
                new Claim(ClaimTypes.Name, credencialesUsuarioDTO.UserName!)
            };

            var usuario = await userManager.FindByNameAsync(credencialesUsuarioDTO.UserName!);
            var roles = await userManager.GetRolesAsync(usuario!);
            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var llave = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(config["LlaveJWT"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(10);

            var TokenSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: credenciales);

            var token = new JwtSecurityTokenHandler().WriteToken(TokenSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion
            };
        }
        private string MapIdentityError(IdentityError error)
        {
            switch (error.Code)
            {
                case "DuplicateUserName":
                    return "Este nombre de usuario ya está en uso.";
                case "DuplicateEmail":
                    return "Ya existe una cuenta registrada con este correo electrónico.";
                case "PasswordTooShort":
                    return "La contraseña es demasiado corta.";
                case "PasswordRequiresNonAlphanumeric":
                    return "La contraseña debe contener al menos un carácter no alfanumérico.";
                case "PasswordRequiresDigit":
                    return "La contraseña debe contener al menos un número.";
                case "PasswordRequiresUpper":
                    return "La contraseña debe contener al menos una letra mayúscula.";
                case "PasswordRequiresLower":
                    return "La contraseña debe contener al menos una letra minúscula.";
                default:
                    return error.Description; // Por defecto, usa el mensaje original
            }
        }

    }
}
