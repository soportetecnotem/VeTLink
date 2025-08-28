using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class CatalogosController(
        ApplicationDbContext context,
        IMapper mapper) : ControllerBase
    {
        //Condicion Corporal
        //Listado
        [HttpGet("CondicionCorporal/Listado")]
        [EndpointSummary("Obtiene la lista de condiciones corporales")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetCondicionesCorporales()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var condiciones = await context.CondicionesCorporal
                    .AsNoTracking()
                    .ToListAsync();

                var condicionesDto = mapper.Map<List<DetalleCatalogoDTO>>(condiciones);

                respuesta.Status = true;
                respuesta.Response = condicionesDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener las condiciones corporales: {ex.Message}");
                return  respuesta;
            }
        }

        // DETALLES
        [HttpGet("CondicionCorporal/{id:int}")]
        [EndpointSummary("Obtiene detalles de una condición corporal por Id")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetCondicionCorporal(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var condicion = await context.CondicionesCorporal.FindAsync(id);
            if (condicion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Condición corporal no encontrada.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(condicion);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("CondicionCorporal/Nuevo")]
        [EndpointSummary("Crea una nueva condición corporal")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearCondicionCorporal(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var condicion = mapper.Map<CondicionCorporal>(dto);
            context.CondicionesCorporal.Add(condicion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(condicion);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("CondicionCorporal/{id:int}")]
        [EndpointSummary("Edita una condición corporal existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarCondicionCorporal(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var condicion = await context.CondicionesCorporal.FindAsync(id);
            if (condicion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Condición corporal no encontrada.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, condicion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(condicion);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("CondicionCorporal/{id:int}")]
        [EndpointSummary("Elimina una condición corporal")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarCondicionCorporal(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var condicion = await context.CondicionesCorporal.FindAsync(id);
            if (condicion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Condición corporal no encontrada.");
                return NotFound(respuesta);
            }

            context.CondicionesCorporal.Remove(condicion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Message.Add("Condición corporal eliminada correctamente.");
            return Ok(respuesta);
        }

        //Estado General
        //Listado
        [HttpGet("EstadoGral/Listado")]
        [EndpointSummary("Obtiene la lista de Estados Generales")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetEstadoGral()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var estados = await context.EstadosGeneral
                    .AsNoTracking()
                    .ToListAsync();

                var estadosDto = mapper.Map<List<DetalleCatalogoDTO>>(estados);

                respuesta.Status = true;
                respuesta.Response = estadosDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener las estados generales: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("EstadoGral/{id:int}")]
        [EndpointSummary("Obtiene detalles de un estado general por Id")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetEstadoGral(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var estado = await context.EstadosGeneral.FindAsync(id);
            if (estado == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Estado General no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(estado);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("EstadoGral/Nuevo")]
        [EndpointSummary("Crea un nuevo estdo general")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearEstadoGral(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var estado = mapper.Map<EstadoGeneral>(dto);
            context.EstadosGeneral.Add(estado);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(estado);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("EstadoGral/{id:int}")]
        [EndpointSummary("Edita un estado general existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarEstadoGral(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var estado = await context.EstadosGeneral.FindAsync(id);
            if (estado == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Estado general no encontrada.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, estado);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(estado);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("EstadoGral/{id:int}")]
        [EndpointSummary("Elimina una condición corporal")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarEstadoGral(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var condicion = await context.CondicionesCorporal.FindAsync(id);
            if (condicion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Condición corporal no encontrada.");
                return NotFound(respuesta);
            }

            context.CondicionesCorporal.Remove(condicion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Message.Add("Condición corporal eliminada correctamente.");
            return Ok(respuesta);
        }
    }
}
