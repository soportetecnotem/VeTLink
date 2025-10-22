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
using VeTLink.DTOs;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context,
        IConfiguration config,
        IMapper mapper) : ControllerBase
    {
        [HttpPost("Nuevo")]
        [EndpointSummary("Registrar nuevo usuario.")]
        public async Task<ActionResult<RespuestaGeneralDTO>> Register(RegisterDto model)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            var user = mapper.Map<IdentityUser>(model);
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var persona = mapper.Map<Persona>(model);
                persona.UsuarioId = user.Id;
                persona.TipoUsuarioId = 3;

                context.Personas.Add(persona);
                await context.SaveChangesAsync();

                //var token = GenerateJwtToken(user);
                
                respuesta.Status = true;
                respuesta.Message.Add("Usuario Creado.");
                //respuesta.Response = token;
                return respuesta;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    var customMessage = MapIdentityError(error);
                    respuesta.Message.Add(customMessage);
                }

                return respuesta;
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

        [HttpPost("PrimerUsuario")]
        public async Task<IActionResult> RegisterFirstUserAdmin(RegisterDto model)
        {
            var user = mapper.Map<IdentityUser>(model);
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

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
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));

                await userManager.AddToRoleAsync(user, "User");
            }

            var token = GenerateJwtTokenAsync(user);
            return Ok(new { Token = token });
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
                var token = GenerateJwtTokenAsync(user);

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

        private async Task<TokenDTO> GenerateJwtTokenAsync(IdentityUser user)
        {
            var jwtSettings = config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new("UserId", user.Id),
                new("Email", user.Email ?? "")
            };

            var usuario = await userManager.FindByEmailAsync(user.Email!);
            var roles = await userManager.GetRolesAsync(usuario!);
            foreach (var rol in roles)
            {
                claims.Add(new Claim("Roles", rol));
            }
            var expiracion = DateTime.UtcNow.AddDays(1);

            var TokenSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                           claims: claims, expires: expiracion, signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(TokenSeguridad);
            return new TokenDTO
            {
                Token = token,
                Expiracion = expiracion
            };
        }

        private async Task<RespuestaAutenticacionDTO> ConstruirToken(
            IdentityUser credencialesUsuarioDTO)
        {
            var claims = new List<Claim>
            {
                new("UserId", credencialesUsuarioDTO.Id),
                new("Email", credencialesUsuarioDTO.Email!),
                new("UserName",credencialesUsuarioDTO.UserName!)
            };
            var usuario = await userManager.FindByNameAsync(credencialesUsuarioDTO.UserName!);
            var roles = await userManager.GetRolesAsync(usuario!);
            foreach (var rol in roles)
            {
                claims.Add(new Claim("Roles", rol));
            }

            //var claimsDB = await userManager.GetClaimsAsync(usuario!);

            //claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(config["LlaveJWT"]!));
            var credemciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(10);

            var TokenSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: credemciales);

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
