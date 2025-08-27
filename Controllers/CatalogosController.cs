using AutoMapper;
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
    public class CatalogosController(
        ApplicationDbContext context,
        IMapper mapper) : ControllerBase
    {
        //Condicion Corporal
        //Listado
        [HttpGet("Listado")]
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
        [HttpGet("{id:int}")]
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
        [HttpPost]
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
        [HttpPut("{id:int}")]
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
        [HttpDelete("{id:int}")]
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
    }
}
