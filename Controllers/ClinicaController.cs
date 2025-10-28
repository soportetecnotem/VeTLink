using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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

        // NUEVA SUCURSAL
        [HttpPost("Sucursal/Nuevo")]
        [EndpointSummary("Crea una nueva sucursal asociada a una clínica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearSucursal(CrearSucursalDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            var clinica = await context.Clinicas.FindAsync(dto.ClinicaId);
            if (clinica == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Clínica no encontrada." };
                return NotFound(respuesta);
            }

            var sucursal = mapper.Map<Sucursal>(dto);
            sucursal.ClinicaId = dto.ClinicaId;

            context.Sucursales.Add(sucursal);
            await context.SaveChangesAsync();

            var detalle = mapper.Map<DetalleSucursalDTO>(sucursal);
            detalle.NombreClinica = clinica.NombreClinica;

            respuesta.Status = true;
            respuesta.Response = detalle;
            respuesta.Message = new() { "Sucursal creada correctamente." };
            return Ok(respuesta);
        }

        // EDITAR SUCURSAL
        [HttpPut("Sucursal/Actualizar/{id:int}")]
        [EndpointSummary("Actualiza los datos de una sucursal existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarSucursal(int id, UpdateSucursalDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            var sucursal = await context.Sucursales
                .Include(s => s.Direccion)
                .Include(s => s.Clinica)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sucursal == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Sucursal no encontrada." };
                return NotFound(respuesta);
            }

            // Actualizar datos principales
            mapper.Map(dto, sucursal);

            // Actualizar dirección si se proporciona
            if (dto.Direccion != null)
            {
                if (sucursal.Direccion == null)
                    sucursal.Direccion = mapper.Map<Direccion>(dto.Direccion);
                else
                    mapper.Map(dto.Direccion, sucursal.Direccion);
            }

            await context.SaveChangesAsync();

            var detalle = mapper.Map<DetalleSucursalDTO>(sucursal);
            detalle.NombreClinica = sucursal.Clinica?.NombreClinica;

            respuesta.Status = true;
            respuesta.Response = detalle;
            respuesta.Message = new() { "Sucursal actualizada correctamente." };
            return Ok(respuesta);
        }

        // DETALLES DE UNA SUCURSAL
        [HttpGet("Sucursal/Detalles/{id:int}")]
        [EndpointSummary("Obtiene los detalles completos de una sucursal")]
        public async Task<ActionResult<RespuestaObjetoDTO>> DetallesSucursal(int id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var sucursal = await context.Sucursales
                .Include(s => s.Direccion)
                .Include(s => s.Clinica)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sucursal == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Sucursal no encontrada." };
                return NotFound(respuesta);
            }

            var detalle = mapper.Map<DetalleSucursalDTO>(sucursal);
            detalle.NombreClinica = sucursal.Clinica?.NombreClinica;

            respuesta.Status = true;
            respuesta.Response = detalle;
            return Ok(respuesta);
        }

        // LISTADO GENERAL DE SUCURSALES
        [HttpGet("Sucursal/Listado")]
        [EndpointSummary("Obtiene el listado de todas las sucursales con su clínica asociada")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ListadoSucursales()
        {
            var respuesta = new RespuestaObjetoDTO();

            var sucursales = await context.Sucursales
                .Include(s => s.Clinica)
                .ToListAsync();

            var listado = mapper.Map<List<ListadoSucursalDTO>>(sucursales);
            foreach (var item in listado)
            {
                var clinica = sucursales.First(s => s.Id == item.Id).Clinica;
                item.NombreClinica = clinica?.NombreClinica;
            }

            respuesta.Status = true;
            respuesta.Response = listado;
            return Ok(respuesta);
        }

        // LISTADO POR CLÍNICA
        [HttpGet("Sucursal/ListadoPorClinica/{clinicaId:int}")]
        [EndpointSummary("Obtiene el listado de sucursales que pertenecen a una clínica específica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ListadoPorClinica(int clinicaId)
        {
            var respuesta = new RespuestaObjetoDTO();

            var sucursales = await context.Sucursales
                .Where(s => s.ClinicaId == clinicaId)
                .Include(s => s.Clinica)
                .ToListAsync();

            if (!sucursales.Any())
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No se encontraron sucursales para la clínica especificada." };
                return NotFound(respuesta);
            }

            var listado = mapper.Map<List<ListadoSucursalDTO>>(sucursales);
            foreach (var item in listado)
            {
                var clinica = sucursales.First(s => s.Id == item.Id).Clinica;
                item.NombreClinica = clinica?.NombreClinica;
            }

            respuesta.Status = true;
            respuesta.Response = listado;
            return Ok(respuesta);
        }
    }    
}
