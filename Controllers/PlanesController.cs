using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using VeTLink.Data;
using VeTLink.DTOs;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PlanesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // POST: api/planes/nuevo
        [HttpPost("nuevo")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Crear([FromBody] PlanCrearDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var plan = _mapper.Map<Plan>(dto);

                // Asignar módulos si se enviaron IDs
                if (dto.ModuloIds?.Any() == true)
                {
                    plan.Modulos = await _context.Modulos
                        .Where(m => dto.ModuloIds.Contains(m.Id))
                        .ToListAsync();
                }

                _context.Planes.Add(plan);
                await _context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message!.Add("Plan creado correctamente");
                respuesta.Response = _mapper.Map<PlanDetalleDTO>(plan);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        // PUT: api/planes/actualizar
        [HttpPut("actualizar")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Actualizar([FromBody] PlanActualizarDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var plan = await _context.Planes
                    .Include(p => p.Modulos)
                    .FirstOrDefaultAsync(p => p.Id == dto.Id);

                if (plan == null)
                {
                    respuesta.Status = false;
                    respuesta.Message!.Add("Plan no encontrado");
                    return NotFound(respuesta);
                }

                _mapper.Map(dto, plan);

                // Actualizar módulos
                if (dto.ModuloIds != null)
                {
                    plan.Modulos = await _context.Modulos
                        .Where(m => dto.ModuloIds.Contains(m.Id))
                        .ToListAsync();
                }

                _context.Planes.Update(plan);
                await _context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message!.Add("Plan actualizado correctamente");
                respuesta.Response = _mapper.Map<PlanDetalleDTO>(plan);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        // GET: api/planes/detalle/{id}
        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Detalle(int id)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var plan = await _context.Planes
                    .Include(p => p.Modulos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (plan == null)
                {
                    respuesta.Status = false;
                    respuesta.Message!.Add("Plan no encontrado");
                    return NotFound(respuesta);
                }

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<PlanDetalleDTO>(plan);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        // GET: api/planes/listado
        [HttpGet("listado")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Listado()
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var planes = await _context.Planes
                    .Include(p => p.Modulos)
                    .ToListAsync();

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<List<PlanDetalleDTO>>(planes);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        // DELETE: api/planes/eliminar/{id}
        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Eliminar(int id)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var plan = await _context.Planes.FindAsync(id);

                if (plan == null)
                {
                    respuesta.Status = false;
                    respuesta.Message!.Add("Plan no encontrado");
                    return NotFound(respuesta);
                }

                _context.Planes.Remove(plan);
                await _context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message!.Add("Plan eliminado correctamente");
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        // ---------------------- MÓDULOS ----------------------

        [HttpPost("modulo/nuevo")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearModulo([FromBody] ModuloDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var modulo = _mapper.Map<Modulo>(dto);
                _context.Modulos.Add(modulo);
                await _context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message!.Add("Módulo creado correctamente");
                respuesta.Response = _mapper.Map<ModuloActualizarDTO>(modulo);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        [HttpPut("modulo/actualizar")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ActualizarModulo([FromBody] ModuloActualizarDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var modulo = await _context.Modulos.FindAsync(dto.Id);

                if (modulo == null)
                {
                    respuesta.Status = false;
                    respuesta.Message!.Add("Módulo no encontrado");
                    return NotFound(respuesta);
                }

                _mapper.Map(dto, modulo);

                _context.Modulos.Update(modulo);
                await _context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message!.Add("Módulo actualizado correctamente");
                respuesta.Response = _mapper.Map<ModuloActualizarDTO>(modulo);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        [HttpGet("modulo/detalle/{id}")]
        public async Task<ActionResult<RespuestaObjetoDTO>> DetalleModulo(int id)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var modulo = await _context.Modulos.FindAsync(id);

                if (modulo == null)
                {
                    respuesta.Status = false;
                    respuesta.Message!.Add("Módulo no encontrado");
                    return NotFound(respuesta);
                }

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<ModuloActualizarDTO>(modulo);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        [HttpGet("modulo/listado")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ListadoModulos()
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var modulos = await _context.Modulos.ToListAsync();

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<List<ModuloActualizarDTO>>(modulos);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }

        [HttpDelete("modulo/eliminar/{id}")]
        public async Task<ActionResult<RespuestaObjetoDTO>> EliminarModulo(int id)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var modulo = await _context.Modulos.FindAsync(id);

                if (modulo == null)
                {
                    respuesta.Status = false;
                    respuesta.Message!.Add("Módulo no encontrado");
                    return NotFound(respuesta);
                }

                _context.Modulos.Remove(modulo);
                await _context.SaveChangesAsync();

                respuesta.Status = true;
                respuesta.Message!.Add("Módulo eliminado correctamente");
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message!.Add($"Error: {ex.Message}");
            }

            return Ok(respuesta);
        }
    }
}
