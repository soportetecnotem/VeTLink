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
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var clinicas = await context.Clinicas
                    .Include(c => c.Suscripcion)
                    .AsNoTracking()
                    .ToListAsync();

                var clinicasDTO = mapper.Map<List<DetalleClinicaDTO>>(clinicas);

                respuesta.Status = true;
                respuesta.Response = clinicasDTO;
                respuesta.Message = new List<string> { "Listado de clínicas obtenido correctamente." };
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new List<string> { "Error al obtener clínicas.", ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
            }
        }

        // Detalles
        [HttpGet("Detalles/{id:int}")]
        [EndpointSummary("Obtiene los detalles de una clínica por ID")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetClinica(int id)
        {
            var clinica = await context.Clinicas
                .Include(c => c.Suscripcion)
                .Include(c => c.Sucursales)
                    .ThenInclude(s => s.Direccion)
                .AsNoTracking()
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
        [EndpointSummary("Actualiza la información de una clínica y sus sucursales")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarClinica(int id, [FromBody] DetalleClinicaDTO dto)
        {
            var clinica = await context.Clinicas
                .Include(c => c.Sucursales)
                    .ThenInclude(s => s.Direccion)
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

            // Actualizar datos de la clínica
            mapper.Map(dto, clinica);

            // Manejo de sucursales
            if (dto.Sucursales != null)
            {
                foreach (var sucursalDto in dto.Sucursales)
                {
                    if (sucursalDto.Id == 0)
                    {
                        // Nueva sucursal
                        var nuevaSucursal = mapper.Map<Sucursal>(sucursalDto);
                        clinica.Sucursales.Add(nuevaSucursal);
                    }
                    else
                    {
                        // Buscar sucursal existente
                        var sucursalExistente = clinica.Sucursales.FirstOrDefault(s => s.Id == sucursalDto.Id);
                        if (sucursalExistente != null)
                        {
                            // Actualizar datos de la sucursal
                            mapper.Map(sucursalDto, sucursalExistente);

                            // Si viene dirección, actualizarla también
                            if (sucursalDto.Direccion != null)
                            {
                                if (sucursalExistente.Direccion == null)
                                {
                                    sucursalExistente.Direccion = mapper.Map<Direccion>(sucursalDto.Direccion);
                                }
                                else
                                {
                                    mapper.Map(sucursalDto.Direccion, sucursalExistente.Direccion);
                                }
                            }
                        }
                    }
                }
            }

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
        [EndpointSummary("Elimina una clínica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarClinica(int id)
        {
            var clinica = await context.Clinicas
                .Include(c => c.Sucursales)
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

            // Validar que no tenga sucursales
            if (clinica.Sucursales.Any())
            {
                return BadRequest(new RespuestaObjetoDTO
                {
                    Status = false,
                    Message = new List<string> { "No se puede eliminar la clínica porque tiene sucursales asociadas" }
                });
            }

            // Validar que no tenga suscripción activa
            if (clinica.SuscripcionId != null)
            {
                return BadRequest(new RespuestaObjetoDTO
                {
                    Status = false,
                    Message = new List<string> { "No se puede eliminar la clínica porque tiene una suscripción asociada" }
                });
            }

            // Si no hay relaciones, se elimina
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
