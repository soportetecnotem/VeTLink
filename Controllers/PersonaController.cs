using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeTLink.Data;
using VeTLink.DTOs.Persona;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonaController(ApplicationDbContext context, IMapper mapper) : ControllerBase
    {
    
        private readonly ApplicationDbContext _context = context;    
        private readonly IMapper _mapper = mapper;

        // LISTADO
        [HttpGet("Listado")]
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

                var dto = _mapper.Map<List<ListadoPersonaDTO>>(personas);

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
        [HttpGet("Detalles/{id:guid}")]
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
        [HttpPost("Nuevo")]
        [EndpointSummary("Crea una nueva persona vinculada a un usuario Identity")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Create(CreatePersonaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            // Validar duplicado por UsuarioId
            var yaExiste = await _context.Personas.AnyAsync(p => p.UsuarioId == dto.UsuarioId);
            if (yaExiste)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Ya existe una Persona vinculada a este UsuarioId." };
                return Conflict(respuesta);
            }

            // Mapear Persona
            var persona = _mapper.Map<Persona>(dto);

            // Mapear Dirección si viene en el DTO
            if (dto.Direccion != null)
            {
                persona.Direccion = _mapper.Map<Direccion>(dto.Direccion);
            }

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            // Reconsultar para incluir todos los datos relacionados
            var creada = await _context.Personas
                .Include(p => p.Usuario)
                .Include(p => p.TipoUsuario)
                .Include(p => p.Direccion) // incluir dirección
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
        [HttpPut("Editar/{id:guid}")]
        [EndpointSummary("Actualiza una persona existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Update(Guid id, CreatePersonaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            var persona = await _context.Personas
                .Include(p => p.Direccion) // incluimos la dirección
                .FirstOrDefaultAsync(p => p.Id == id);

            if (persona == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Persona no encontrada." };
                return NotFound(respuesta);
            }

            // Actualizamos la persona con AutoMapper
            _mapper.Map(dto, persona);

            // Si el DTO trae dirección, actualizamos también
            if (dto.Direccion != null)
            {
                if (persona.Direccion == null)
                {
                    // Si no tiene dirección, la creamos
                    persona.Direccion = _mapper.Map<Direccion>(dto.Direccion);
                }
                else
                {
                    // Si ya existe, actualizamos sus campos
                    _mapper.Map(dto.Direccion, persona.Direccion);
                }
            }

            await _context.SaveChangesAsync();

            var actualizada = await _context.Personas
                .Include(p => p.Usuario)
                .Include(p => p.TipoUsuario)
                .Include(p => p.Direccion) //incluir dirección para el retorno
                .AsNoTracking()
                .FirstAsync(p => p.Id == id);

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<DetallePersonaDTO>(actualizada);
            respuesta.Message = new() { "Persona actualizada correctamente." };
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Eliminar/{id:guid}")]
        [EndpointSummary("Elimina una persona por Id")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Delete(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var persona = await _context.Personas
            .Include(p => p.Direccion) // cargamos la dirección
            .FirstOrDefaultAsync(p => p.Id == id);

            if (persona == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Persona no encontrada." };
                return NotFound(respuesta);
            }

            // Si tiene dirección asociada la eliminamos también
            if (persona.Direccion != null)
            {
                _context.Direcciones.Remove(persona.Direccion);
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();

            respuesta.Status = true;
            respuesta.Message = new() { "Persona eliminada correctamente." };
            return Ok(respuesta);
        }
    }
}
