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
        [Authorize]
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

        [HttpPost("login")]
        [EndpointSummary("Obtiene token al ingresar las credenciales correctas")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Login(LoginDto model)
        {
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

                var token = GenerateJwtToken(user);
                respuesta.Status = true;
                respuesta.Message.Add("Login correcto.");
                respuesta.Response = new
                {
                    Token = token,
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

            var token = GenerateJwtToken(user);
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
                var token = GenerateJwtToken(user);

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

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
