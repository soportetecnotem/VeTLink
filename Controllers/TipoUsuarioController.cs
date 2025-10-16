using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeTLink.Data;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TipoUsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TipoUsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTADO
        [HttpGet("Listado")]
        [EndpointSummary("Obtiene todos los tipos de usuario (Médico, Dueño)")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetAll()
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var tipos = await _context.TiposUsuarios.AsNoTracking().ToListAsync();

                respuesta.Status = true;
                respuesta.Response = tipos;
                respuesta.Message = new() { "Listado obtenido correctamente." };
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Error al obtener listado.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        // DETALLE
        [HttpGet("Detalles/{id:int}")]
        [EndpointSummary("Obtiene los detalles de un tipo de usuario por Id")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetById(int id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var tipo = await _context.TiposUsuarios.FindAsync(id);
            if (tipo == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Tipo de usuario no encontrado." };
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = tipo;
            respuesta.Message = new() { "Detalle obtenido correctamente." };
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost("Nuevo")]
        [EndpointSummary("Crea un nuevo tipo de usuario (Médico, Dueño)")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Create([FromBody] TipoUsuario model)
        {
            var respuesta = new RespuestaObjetoDTO();

            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                respuesta.Status = false;
                respuesta.Message = new() { "El nombre es obligatorio." };
                return BadRequest(respuesta);
            }

            _context.TiposUsuarios.Add(model);
            await _context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = model;
            respuesta.Message = new() { "Tipo de usuario creado correctamente." };
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, respuesta);
        }

        // ACTUALIZAR
        [HttpPut("Editar/{id:int}")]
        [EndpointSummary("Actualiza un tipo de usuario existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Update(int id, [FromBody] TipoUsuario model)
        {
            var respuesta = new RespuestaObjetoDTO();

            var tipo = await _context.TiposUsuarios.FindAsync(id);
            if (tipo == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Tipo de usuario no encontrado." };
                return NotFound(respuesta);
            }

            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                respuesta.Status = false;
                respuesta.Message = new() { "El nombre es obligatorio." };
                return BadRequest(respuesta);
            }

            tipo.Nombre = model.Nombre;
            await _context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Response = tipo;
            respuesta.Message = new() { "Tipo de usuario actualizado correctamente." };
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Eliminar/{id:int}")]
        [EndpointSummary("Elimina un tipo de usuario por Id")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Delete(int id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var tipo = await _context.TiposUsuarios.FindAsync(id);
            if (tipo == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Tipo de usuario no encontrado." };
                return NotFound(respuesta);
            }

            _context.TiposUsuarios.Remove(tipo);
            await _context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Message = new() { "Tipo de usuario eliminado correctamente." };
            return Ok(respuesta);
        }
    }
}
