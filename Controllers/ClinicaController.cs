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
    public class ClinicaController(ApplicationDbContext context,
        UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext context = context;
        private readonly UserManager<IdentityUser> userManager = userManager;
        private readonly RoleManager<IdentityRole> roleManager = roleManager;
        private readonly IMapper mapper = mapper;
    

    // Listado
        [HttpGet("Listado")]
        [EndpointSummary("Listado de clinicas")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetClinicas()
        {
            var clinicas = await context.Clinicas
                .Include(c => c.Direccion)
                .Include(c => c.Suscripcion)
                .ToListAsync();

            var clinicasDTO = mapper.Map<List<DetalleClinicaDTO>>(clinicas);

            return new RespuestaObjetoDTO
            {
                Status = true,
                Response = clinicasDTO
            };
        }

        // Detalles
        [HttpGet("Detalles/{id:int}")]
        [EndpointSummary("Obtiene los detalles de una clinica por ID")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetClinica(int id)
        {
            var clinica = await context.Clinicas
                .Include(c => c.Direccion)
                .Include(c => c.Suscripcion)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinica == null)
            {
                return NotFound(new RespuestaObjetoDTO
                {
                    Status = false,
                    Message = new List<string> { "Clínica no encontrada" }
                });
            }

            var dto = mapper.Map<DetalleClinicaDTO>(clinica);

            return new RespuestaObjetoDTO
            {
                Status = true,
                Response = dto
            };
        }

        // Nuevo
        [HttpPost("Nuevo")]
        [EndpointSummary("Crea un nuevo registro de clinica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearClinica([FromBody] ClinicaDTO dto)
        {
            var clinica = mapper.Map<Clinica>(dto);
            clinica.Activo = true;

            context.Clinicas.Add(clinica);
            await context.SaveChangesAsync();

            var detalleDto = mapper.Map<DetalleClinicaDTO>(clinica);

            return new RespuestaObjetoDTO
            {
                Status = true,
                Response = detalleDto,
                Message = new List<string> { "Clínica creada exitosamente" }
            };
        }

        // Actualizar
        [HttpPut("Actualizar/{id:int}")]
        [EndpointSummary("Actualiza la informacion de una clinica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarClinica(int id, [FromBody] ClinicaDTO dto)
        {
            var clinica = await context.Clinicas
                .Include(c => c.Direccion)
                .Include(c => c.Suscripcion)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinica == null)
            {
                return NotFound(new RespuestaObjetoDTO
                {
                    Status = false,
                    Message = new List<string> { "Clínica no encontrada" }
                });
            }

            mapper.Map(dto, clinica);
            await context.SaveChangesAsync();

            var detalleDto = mapper.Map<DetalleClinicaDTO>(clinica);

            return new RespuestaObjetoDTO
            {
                Status = true,
                Response = detalleDto,
                Message = new List<string> { "Clínica actualizada exitosamente" }
            };
        }

        // DELETE
        [HttpDelete("Eliminar/{id:int}")]
        [EndpointSummary("Elimina una clinica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarClinica(int id)
        {
            var clinica = await context.Clinicas
                .Include(c => c.Direccion)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinica == null)
            {
                return NotFound(new RespuestaObjetoDTO
                {
                    Status = false,
                    Message = new List<string> { "Clínica no encontrada" }
                });
            }

            // Si la clínica tiene dirección, la eliminamos primero
            if (clinica.Direccion != null)
            {
                context.Direcciones.Remove(clinica.Direccion);
            }

            context.Clinicas.Remove(clinica);
            await context.SaveChangesAsync();

            return new RespuestaObjetoDTO
            {
                Status = true,
                Message = new List<string> { "Clínica eliminada exitosamente" }
            };
        }
    }    
}
