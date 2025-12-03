using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using VeTLink.Data;
using VeTLink.DTOs.Catalogo;
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
        [EndpointSummary("Obtiene la lista de las condiciones corporales.")]
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
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return  respuesta;
            }
        }

        // DETALLES
        [HttpGet("CondicionCorporal/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de una condición corporal por Id.")]
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
        [EndpointSummary("Crea una nueva condición corporal.")]
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
        [HttpPut("CondicionCorporal/Editar{id:int}")]
        [EndpointSummary("Edita una condición corporal existente.")]
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
        [HttpDelete("CondicionCorporal/Eliminar/{id:int}")]
        [EndpointSummary("Elimina una condición corporal.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarCondicionCorporal(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            { 
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
            respuesta.Message.Add("Elemento eliminado correctamente.");
            return Ok(respuesta);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //Estado General
        //Listado
        [HttpGet("EstadoGral/Listado")]
        [EndpointSummary("Obtiene la lista de los Estados Generales.")]
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
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("EstadoGral/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un estado general por Id.")]
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
        [EndpointSummary("Crea un nuevo estado general.")]
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
        [HttpPut("EstadoGral/Editar/{id:int}")]
        [EndpointSummary("Edita un estado general existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarEstadoGral(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var estado = await context.EstadosGeneral.FindAsync(id);
            if (estado == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Estado general no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, estado);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(estado);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("EstadoGral/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un estado general.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarEstadoGral(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try 
            {
            var estadoGeneral = await context.EstadosGeneral.FindAsync(id);
            if (estadoGeneral == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            context.EstadosGeneral.Remove(estadoGeneral);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Message.Add("Elemento eliminado correctamente.");
            return Ok(respuesta);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //Mucosas
        //Listado
        [HttpGet("Mucosas/Listado")]
        [EndpointSummary("Obtiene la lista de Mucosas.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetMucosas()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var mucosas = await context.Mucosas
                    .AsNoTracking()
                    .ToListAsync();

                var mucosasDto = mapper.Map<List<DetalleCatalogoDTO>>(mucosas);

                respuesta.Status = true;
                respuesta.Response = mucosasDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("Mucosas/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de una mucosa por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetMucosas(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var mucosas = await context.Mucosas.FindAsync(id);
            if (mucosas == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(mucosas);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("Mucosas/Nuevo")]
        [EndpointSummary("Crea una nueva mucosa.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearMucosas(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var mucosa = mapper.Map<Mucosa>(dto);
            context.Mucosas.Add(mucosa);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(mucosa);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("Mucosas/Editar/{id:int}")]
        [EndpointSummary("Edita una mucosa existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarMucosas(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var mucosa = await context.Mucosas.FindAsync(id);
            if (mucosa == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, mucosa);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(mucosa);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Mucosas/Eliminar/{id:int}")]
        [EndpointSummary("Elimina una mucosa por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarMucosas(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var mucosa = await context.Mucosas.FindAsync(id);
                if (mucosa == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.Mucosas.Remove(mucosa);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //Hidratacion
        //Listado
        [HttpGet("Hidratacion/Listado")]
        [EndpointSummary("Obtiene la lista de las Hidrataciones.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetHidratacion()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var hidratacion = await context.Hidrataciones
                    .AsNoTracking()
                    .ToListAsync();

                var hidratacionDto = mapper.Map<List<DetalleCatalogoDTO>>(hidratacion);

                respuesta.Status = true;
                respuesta.Response = hidratacionDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("Hidratacion/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de una Hidratacion por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetHidratacion(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var hidratacion = await context.Hidrataciones.FindAsync(id);
            if (hidratacion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(hidratacion);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("Hidratacion/Nuevo")]
        [EndpointSummary("Crea una nueva Hidratacion.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearHidratacion(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var hidratacion = mapper.Map<Hidratacion>(dto);
            context.Hidrataciones.Add(hidratacion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(hidratacion);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("Hidratacion/Editar/{id:int}")]
        [EndpointSummary("Edita una Hidratacion existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarHidratacion(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var hidratacion = await context.Hidrataciones.FindAsync(id);
            if (hidratacion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, hidratacion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(hidratacion);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Hidratacion/Eliminar/{id:int}")]
        [EndpointSummary("Elimina una Hidratacion por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarHidratacion(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            { 
            var hidratacion = await context.Hidrataciones.FindAsync(id);
            if (hidratacion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            context.Hidrataciones.Remove(hidratacion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Message.Add("Elemento eliminado correctamente.");
            return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //Unidad de Tiempo
        //Listado
        [HttpGet("UnidadTiempo/Listado")]
        [EndpointSummary("Obtiene la lista de las Unidades de Tiempo.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetUnidadTiempo()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var unidadTiempo = await context.UnidadesTiempo
                    .AsNoTracking()
                    .ToListAsync();

                var unidadTiempoDto = mapper.Map<List<DetalleCatalogoDTO>>(unidadTiempo);

                respuesta.Status = true;
                respuesta.Response = unidadTiempoDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("UnidadTiempo/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de una unidad de Tiempo por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetUnidadTiempo(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var unidadTiempo = await context.UnidadesTiempo.FindAsync(id);
            if (unidadTiempo == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(unidadTiempo);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("UnidadTiempo/Nuevo")]
        [EndpointSummary("Crea una nueva Unidad de Tiempo.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearUnidadTiempo(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var unidadTiempo = mapper.Map<UnidadTiempo>(dto);
            context.UnidadesTiempo.Add(unidadTiempo);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(unidadTiempo);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("UnidadTiempo/Editar/{id:int}")]
        [EndpointSummary("Edita una Unidad de Tiempo existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarUnidadTiempo(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var unidadTiempo = await context.UnidadesTiempo.FindAsync(id);
            if (unidadTiempo == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, unidadTiempo);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(unidadTiempo);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("UnidadTiempo/Eliminar/{id:int}")]
        [EndpointSummary("Elimina una Unidad de Tiempo por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarUnidadTiempo(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var unidadTiempo = await context.UnidadesTiempo.FindAsync(id);
                if (unidadTiempo == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.UnidadesTiempo.Remove(unidadTiempo);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //Comportamiento
        //Listado
        [HttpGet("Comportamiento/Listado")]
        [EndpointSummary("Obtiene la lista de los comportamientos.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetComportamiento()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var comportamiento = await context.Comportamientos
                    .AsNoTracking()
                    .ToListAsync();

                var comportamientoDto = mapper.Map<List<DetalleCatalogoDTO>>(comportamiento);

                respuesta.Status = true;
                respuesta.Response = comportamientoDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("Comportamiento/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un comportamiento por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetComportamientoo(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var comportamiento = await context.Comportamientos.FindAsync(id);
            if (comportamiento == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(comportamiento);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("Comportamiento/Nuevo")]
        [EndpointSummary("Crea un nuevo comportamiento.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearComportamiento(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var comportamiento = mapper.Map<Comportamiento>(dto);
            context.Comportamientos.Add(comportamiento);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(comportamiento);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("Comportamiento/Editar/{id:int}")]
        [EndpointSummary("Edita un comportamiento existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarComportamiento(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var comportamiento = await context.Comportamientos.FindAsync(id);
            if (comportamiento == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, comportamiento);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(comportamiento);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Comportamiento/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un comportamiento por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarComportamiento(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var comportamiento = await context.Comportamientos.FindAsync(id);
                if (comportamiento == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.Comportamientos.Remove(comportamiento);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //tipoPrueba
        //Listado
        [HttpGet("TipoPrueba/Listado")]
        [EndpointSummary("Obtiene la lista de los tipos de Pruebas.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetTipoPrueba()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var tipoPrueba = await context.TiposPruebas
                    .AsNoTracking()
                    .ToListAsync();

                var tipoPruebaDto = mapper.Map<List<DetalleCatalogoDTO>>(tipoPrueba);

                respuesta.Status = true;
                respuesta.Response = tipoPruebaDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener las unidades: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("TipoPrueba/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un tipo de Prueba por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetTipoPrueba(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoPrueba = await context.TiposPruebas.FindAsync(id);
            if (tipoPrueba == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoPrueba);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("TipoPrueba/Nuevo")]
        [EndpointSummary("Crea un nuevo TipoPrueba")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearTipoPrueba(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoPrueba = mapper.Map<TipoPrueba>(dto);
            context.TiposPruebas.Add(tipoPrueba);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoPrueba);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("TipoPrueba/Editar/{id:int}")]
        [EndpointSummary("Edita un tipo de Prueba existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarTipoPrueba(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoPrueba = await context.TiposPruebas.FindAsync(id);
            if (tipoPrueba == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, tipoPrueba);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoPrueba);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("TipoPrueba/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un tipo de Prueba por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarTipoPrueba(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var tipoPrueba = await context.TiposPruebas.FindAsync(id);
                if (tipoPrueba == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.TiposPruebas.Remove(tipoPrueba);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //viaAdministracion
        //Listado
        [HttpGet("ViaAdministracion/Listado")]
        [EndpointSummary("Obtiene la lista de las vias de Administracion.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetViaAdministracion()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var viaAdministracion = await context.ViasAdministracion
                    .AsNoTracking()
                    .ToListAsync();

                var viaAdministracionDto = mapper.Map<List<DetalleCatalogoDTO>>(viaAdministracion);

                respuesta.Status = true;
                respuesta.Response = viaAdministracionDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elelmentos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("ViaAdministracion/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de una via de Administracion por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetViaAdministracion(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var viaAdministracion = await context.ViasAdministracion.FindAsync(id);
            if (viaAdministracion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(viaAdministracion);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("ViaAdministracion/Nuevo")]
        [EndpointSummary("Crea una nueva via de Administracion.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearViaAdministracion(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var viaAdministracion = mapper.Map<ViaAdministracion>(dto);
            context.ViasAdministracion.Add(viaAdministracion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(viaAdministracion);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("ViaAdministracion/Editar/{id:int}")]
        [EndpointSummary("Edita un tipo de una via de Administracion existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarViaAdministracion(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var viaAdministracion = await context.ViasAdministracion.FindAsync(id);
            if (viaAdministracion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, viaAdministracion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(viaAdministracion);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("ViaAdministracion/Eliminar/{id:int}")]
        [EndpointSummary("Elimina una via de Administracion por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarViaAdministracion(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var viaAdministracion = await context.ViasAdministracion.FindAsync(id);
                if (viaAdministracion == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.ViasAdministracion.Remove(viaAdministracion);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

    //tipoTratamiento
        //Listado
        [HttpGet("TipoTratamiento/Listado")]
        [EndpointSummary("Obtiene la lista de los tipos de Tratamiento.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetTipoTratamiento()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var tipoTratamiento = await context.TiposTratamientos
                    .AsNoTracking()
                    .ToListAsync();

                var tipoTratamientoDto = mapper.Map<List<DetalleCatalogoDTO>>(tipoTratamiento);

                respuesta.Status = true;
                respuesta.Response = tipoTratamientoDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elelmentos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("TipoTratamiento/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un tipo de Tratamiento por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetTipoTratamiento(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoTratamiento = await context.TiposTratamientos.FindAsync(id);
            if (tipoTratamiento == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoTratamiento);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("TipoTratamiento/Nuevo")]
        [EndpointSummary("Crea un nuevo tipo de Tratamiento.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearTipoTratamiento(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoTratamiento = mapper.Map<TipoTratamiento>(dto);
            context.TiposTratamientos.Add(tipoTratamiento);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoTratamiento);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("TipoTratamiento/Editar/{id:int}")]
        [EndpointSummary("Edita un tipo de Tratamiento existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarTipoTratamiento(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoTratamiento = await context.TiposTratamientos.FindAsync(id);
            if (tipoTratamiento == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, tipoTratamiento);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoTratamiento);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("TipoTratamiento/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un tipo de Tratamiento por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarTipoTratamiento(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var tipoTratamiento = await context.TiposTratamientos.FindAsync(id);
                if (tipoTratamiento == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.TiposTratamientos.Remove(tipoTratamiento);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //nivelActividad
        //Listado
        [HttpGet("NivelActividad/Listado")]
        [EndpointSummary("Obtiene la lista de los niveles de Actividad.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetNivelActividad()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var nivelActividad = await context.NivelActividades
                    .AsNoTracking()
                    .ToListAsync();

                var nivelActividadDto = mapper.Map<List<DetalleCatalogoDTO>>(nivelActividad);

                respuesta.Status = true;
                respuesta.Response = nivelActividadDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elelmentos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("NivelActividad/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un nivel de Actividad por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetNivelActividad(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var nivelActividad = await context.NivelActividades.FindAsync(id);
            if (nivelActividad == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(nivelActividad);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("NivelActividad/Nuevo")]
        [EndpointSummary("Crea un nuevo nivel de Actividad.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearNivelActividad(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var nivelActividad = mapper.Map<NivelActividad>(dto);
            context.NivelActividades.Add(nivelActividad);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(nivelActividad);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("NivelActividad/Editar/{id:int}")]
        [EndpointSummary("Edita un nivel de Actividad existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarNivelActividad(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var nivelActividad = await context.NivelActividades.FindAsync(id);
            if (nivelActividad == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, nivelActividad);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(nivelActividad);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("NivelActividad/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un nivel de Actividad por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarNivelActividad(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var nivelActividad = await context.NivelActividades.FindAsync(id);
                if (nivelActividad == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.NivelActividades.Remove(nivelActividad);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //reaccionSocial
        //Listado
        [HttpGet("ReaccionSocial/Listado")]
        [EndpointSummary("Obtiene la lista de las reacciones Sociales.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetReaccionSocial()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var reaccionSocial = await context.ReaccionesSociales
                    .AsNoTracking()
                    .ToListAsync();

                var reaccionSocialDto = mapper.Map<List<DetalleCatalogoDTO>>(reaccionSocial);

                respuesta.Status = true;
                respuesta.Response = reaccionSocialDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elelmentos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("ReaccionSocial/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de una reaccion Social por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetReaccionSocial(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var reaccionSocial = await context.ReaccionesSociales.FindAsync(id);
            if (reaccionSocial == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(reaccionSocial);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("ReaccionSocial/Nuevo")]
        [EndpointSummary("Crea una nueva reaccion Social.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearReaccionSocial(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var reaccionSocial = mapper.Map<ReaccionSocial>(dto);
            context.ReaccionesSociales.Add(reaccionSocial);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(reaccionSocial);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("ReaccionSocial/Editar/{id:int}")]
        [EndpointSummary("Edita una reaccion Social existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarReaccionSocial(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var reaccionSocial = await context.ReaccionesSociales.FindAsync(id);
            if (reaccionSocial == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, reaccionSocial);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(reaccionSocial);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("ReaccionSocial/Eliminar/{id:int}")]
        [EndpointSummary("Elimina una reaccion Social por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarReaccionSocial(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var reaccionSocial = await context.ReaccionesSociales.FindAsync(id);
                if (reaccionSocial == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.ReaccionesSociales.Remove(reaccionSocial);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //tipoServicio
        //Listado
        [HttpGet("TipoServicio/Listado")]
        [EndpointSummary("Obtiene la lista de los tipos de Servicio.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetTipoServicio()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var tipoServicio = await context.TiposServicios
                    .AsNoTracking()
                    .ToListAsync();

                var tipoServicioDto = mapper.Map<List<DetalleCatalogoDTO>>(tipoServicio);

                respuesta.Status = true;
                respuesta.Response = tipoServicioDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elelmentos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("TipoServicio/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un tipo de Servicio por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetTipoServicio(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoServicio = await context.TiposServicios.FindAsync(id);
            if (tipoServicio == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoServicio);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("TipoServicio/Nuevo")]
        [EndpointSummary("Crea un nuevo tipo de Servicio.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearTipoServicio(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoServicio = mapper.Map<TipoServicio>(dto);
            context.TiposServicios.Add(tipoServicio);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoServicio);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("TipoServicio/Editar/{id:int}")]
        [EndpointSummary("Edita un tipo de Servicio existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarTipoServicio(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var tipoServicio = await context.TiposServicios.FindAsync(id);
            if (tipoServicio == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, tipoServicio);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(tipoServicio);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("TipoServicio/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un tipo de Servicio por id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarTipoServicio(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var tipoServicio = await context.TiposServicios.FindAsync(id);
                if (tipoServicio == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.TiposServicios.Remove(tipoServicio);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //Estado Suscripcion
        //Listado
        [HttpGet("EstadoSuscripcion/Listado")]
        [EndpointSummary("Obtiene la lista de los Estados de Suscripcion.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetEstadoSuscripcion()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var EstadoSuscripcion = await context.EstadosSuscripcion
                    .AsNoTracking()
                    .ToListAsync();

                var EstadoSuscripcionDto = mapper.Map<List<DetalleCatalogoDTO>>(EstadoSuscripcion);

                respuesta.Status = true;
                respuesta.Response = EstadoSuscripcionDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("EstadoSuscripcion/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un Estado de Suscripcion por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetEstadoSuscripcion(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var EstadoSuscripcion = await context.EstadosSuscripcion.FindAsync(id);
            if (EstadoSuscripcion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(EstadoSuscripcion);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("EstadoSuscripcion/Nuevo")]
        [EndpointSummary("Crea un nuevo Estado de Suscripcion.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearEstadoSuscripcion(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var EstadoSuscripcion = mapper.Map<EstadoSuscripcion>(dto);
            context.EstadosSuscripcion.Add(EstadoSuscripcion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(EstadoSuscripcion);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("EstadoSuscripcion/Editar{id:int}")]
        [EndpointSummary("Edita un Estado de Suscripcion existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarEstadoSuscripcion(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var EstadoSuscripcion = await context.EstadosSuscripcion.FindAsync(id);
            if (EstadoSuscripcion == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, EstadoSuscripcion);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(EstadoSuscripcion);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("EstadoSuscripcion/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un Estado de Suscripcion.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarEstadoSuscripcion(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var EstadoSuscripcion = await context.EstadosSuscripcion.FindAsync(id);
                if (EstadoSuscripcion == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.EstadosSuscripcion.Remove(EstadoSuscripcion);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }

        //Planes
        //Listado
        [HttpGet("Planes/Listado")]
        [EndpointSummary("Obtiene la lista de los Planes.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetPlanes()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                var Planes = await context.Planes
                    .AsNoTracking()
                    .ToListAsync();

                var PlanesDto = mapper.Map<List<DetalleCatalogoDTO>>(Planes);

                respuesta.Status = true;
                respuesta.Response = PlanesDto;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message.Add($"Error al obtener los elementos: {ex.Message}");
                return respuesta;
            }
        }

        // DETALLES
        [HttpGet("Planes/Detalles/{id:int}")]
        [EndpointSummary("Obtiene detalles de un Plan por Id.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetPlanes(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var Planes = await context.Planes.FindAsync(id);
            if (Planes == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(Planes);
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("Planes/Nuevo")]
        [EndpointSummary("Crea un nuevo Plan.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearPlan(CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var Planes = mapper.Map<Plan>(dto);
            context.Planes.Add(Planes);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(Planes);
            return Ok(respuesta);
        }

        // EDITAR
        [HttpPut("Planes/Editar{id:int}")]
        [EndpointSummary("Edita un Plan existente.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EditarPlanes(int id, CatalogoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };

            var Planes = await context.Planes.FindAsync(id);
            if (Planes == null)
            {
                respuesta.Status = false;
                respuesta.Message.Add("Elemento no encontrado.");
                return NotFound(respuesta);
            }

            mapper.Map(dto, Planes);
            await context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = mapper.Map<DetalleCatalogoDTO>(Planes);
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Planes/Eliminar/{id:int}")]
        [EndpointSummary("Elimina un Plan.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarPlanes(int id)
        {
            var respuesta = new RespuestaObjetoDTO { Message = [] };
            try
            {
                var Planes = await context.Planes.FindAsync(id);
                if (Planes == null)
                {
                    respuesta.Status = false;
                    respuesta.Message.Add("Elemento no encontrado.");
                    return NotFound(respuesta);
                }

                context.Planes.Remove(Planes);
                await context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message.Add("Elemento eliminado correctamente.");
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    respuesta.Message.Add("El elemento tiene datos vinculados. Eliminación no permitida.");
                }
                else
                {
                    respuesta.Message.Add(ex.Message);
                }
                return respuesta;
            }
        }
    }
}
