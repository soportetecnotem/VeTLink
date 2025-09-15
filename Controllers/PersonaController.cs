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
    public class PersonaController(ApplicationDbContext context, IMapper mapper) : ControllerBase
    {
    
        private readonly ApplicationDbContext _context = context;    
        private readonly IMapper _mapper = mapper;

        // LISTADO
        [HttpGet]
        [EndpointSummary("Lista de personas")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetAll()
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var personas = await _context.Personas
                    .Include(p => p.Usuario)
                    .Include(p => p.TipoUsuario)
                    .AsNoTracking()
                    .ToListAsync();

                var dto = _mapper.Map<List<DetallePersonaDTO>>(personas);

                respuesta.Status = true;
                respuesta.Response = dto;
                respuesta.Message = new() { "Listado obtenido correctamente." };
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Error al obtener el listado.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        // DETALLES
        [HttpGet("{id:guid}")]
        [EndpointSummary("Detalle de una persona por Id")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetById(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var persona = await _context.Personas
                .Include(p => p.Usuario)
                .Include(p => p.TipoUsuario)
                .Include(p => p.Direccion)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (persona == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Persona no encontrada." };
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<DetallePersonaDTO>(persona);
            respuesta.Message = new() { "Detalle obtenido correctamente." };
            return Ok(respuesta);
        }

        // CREAR
        [HttpPost]
        [EndpointSummary("Crea una nueva persona vinculada a un usuario Identity")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Create(CreatePersonaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            // Validar duplicado por UsuarioId (opcional pero recomendable)
            var yaExiste = await _context.Personas.AnyAsync(p => p.UsuarioId == dto.UsuarioId);
            if (yaExiste)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Ya existe una Persona vinculada a este UsuarioId." };
                return Conflict(respuesta);
            }

            var persona = _mapper.Map<Persona>(dto);
            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            // Reconsultar para mapear Email y TipoUsuarioNombre
            var creada = await _context.Personas
                .Include(p => p.Usuario)
                .Include(p => p.TipoUsuario)
                .AsNoTracking()
                .FirstAsync(p => p.Id == persona.Id);

            var detalle = _mapper.Map<DetallePersonaDTO>(creada);

            var createdRespuesta = new RespuestaObjetoDTO
            {
                Status = true,
                Response = detalle,
                Message = new() { "Persona creada correctamente." }
            };

            return CreatedAtAction(nameof(GetById), new { id = persona.Id }, createdRespuesta);
        }

        // ACTUALIZAR
        [HttpPut("{id:guid}")]
        [EndpointSummary("Actualiza una persona existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Update(Guid id, CreatePersonaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            var persona = await _context.Personas.FirstOrDefaultAsync(p => p.Id == id);
            if (persona == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Persona no encontrada." };
                return NotFound(respuesta);
            }

            _mapper.Map(dto, persona);
            await _context.SaveChangesAsync();

            var actualizada = await _context.Personas
                .Include(p => p.Usuario)
                .Include(p => p.TipoUsuario)
                .AsNoTracking()
                .FirstAsync(p => p.Id == id);

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<DetallePersonaDTO>(actualizada);
            respuesta.Message = new() { "Persona actualizada correctamente." };
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("{id:guid}")]
        [EndpointSummary("Elimina una persona por Id")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Delete(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Persona no encontrada." };
                return NotFound(respuesta);
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Message = new() { "Persona eliminada correctamente." };
            return Ok(respuesta);
        }
    }
}
