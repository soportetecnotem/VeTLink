using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeTLink.Data;
using VeTLink.DTOs;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministracionController(ApplicationDbContext context, 
        UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;

        [HttpPost("registrar")]
        [EndpointSummary("Registra una clínica con su primer veterinario administrador")]
        public async Task<ActionResult<RespuestaObjetoDTO>> RegistrarClinica([FromBody] RegistroClinicaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Mapeo de clínica
                var clinica = _mapper.Map<Clinica>(dto);
                _context.Clinicas.Add(clinica);
                await _context.SaveChangesAsync();

                // 2. Usuario Identity
                var usuario = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email
                };

                var result = await _userManager.CreateAsync(usuario, dto.Password);
                if (!result.Succeeded)
                {
                    respuesta.Status = false;
                    respuesta.Message = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(respuesta);
                }

                // 3. Persona
                var persona = _mapper.Map<Persona>(dto);
                persona.UsuarioId = usuario.Id;
                persona.Usuario = usuario;
                persona.TipoUsuarioId =  1 ; // 1 = Veterinario

                // Calcular edad si la fecha de nacimiento es válida
                if (dto.FechaNacimiento.HasValue && dto.FechaNacimiento.Value != default)
                {
                    var today = DateTime.Today;
                    var edad = today.Year - dto.FechaNacimiento.Value.Year;

                    // Ajustar si aún no ha cumplido años este año
                    if (dto.FechaNacimiento.Value.Date > today.AddYears(-edad))
                    {
                        edad--;
                    }

                    persona.Edad = edad >= 0 ? edad : null; // si resultara negativo, lo dejamos nulo
                }
                else
                {
                    persona.Edad = null; // no asignamos nada si la fecha no es válida
                }

                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                // 4. Veterinario (admin inicial)
                if (dto.EsVeterinario)
                {
                    var veterinario = _mapper.Map<Veterinario>(dto);
                    veterinario.Persona = persona;
                    veterinario.ClinicasAsignadas.Add(clinica);

                    _context.Veterinarios.Add(veterinario);

                    // 5. Verificar si es el primer veterinario de la clínica
                    var totalVeterinariosEnClinica = _context.Veterinarios
                        .Where(v => v.ClinicasAsignadas.Any(c => c.Id == clinica.Id))
                        .Count();

                    if (totalVeterinariosEnClinica == 1) // es el primer veterinario
                    {
                        // Aseguramos que exista el rol "AdminClinica"
                        if (!await _roleManager.RoleExistsAsync("AdminClinica"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("AdminClinica"));
                        }
                    }
                else
                {
                    // Solo persona (no veterinario)
                    await _userManager.AddToRoleAsync(usuario, "AdminClinica");
                }

                await _context.SaveChangesAsync();
                              

                    // Asignamos el rol al usuario de la persona asociada
                    var usuarioV = await _userManager.FindByIdAsync(persona.UsuarioId);
                    if (usuarioV != null)
                    {
                        await _userManager.AddToRoleAsync(usuarioV, "AdminClinica");
                    }
                }

                await transaction.CommitAsync();

                respuesta.Status = true;
                respuesta.Response = new
                {
                    Clinica = _mapper.Map<ClinicaDTO>(clinica),
                    Veterinario = _mapper.Map<VeterinarioDTO>(veterinario)
                };
                respuesta.Message = new() { "Clínica y veterinario administrador registrados correctamente." };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                respuesta.Status = false;
                respuesta.Message = new() { "Error en el registro", ex.Message };
                return StatusCode(500, respuesta);
            }
        }
    }
}
