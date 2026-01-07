using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class PersonaController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager) : ControllerBase
    {
    
        private readonly ApplicationDbContext _context = context;    
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<IdentityUser> _userManager = userManager;

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
            try
            {
                // Obtener el usuario autenticado
                var usuarioAutenticado = await _userManager.GetUserAsync(User);
                if (usuarioAutenticado == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Usuario no autenticado." };
                    return Unauthorized(respuesta);
                }

                // Validar que el usuario autenticado no tenga ya una persona vinculada
                var yaExiste = await _context.Personas.AnyAsync(p => p.UsuarioId == usuarioAutenticado.Id);
                if (yaExiste)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Ya existe una Persona vinculada a este usuario." };
                    return Conflict(respuesta);
                }

                // Mapear Persona
                var persona = _mapper.Map<Persona>(dto);

                // Asignar automáticamente el UsuarioId del usuario autenticado
                persona.UsuarioId = usuarioAutenticado.Id;

                // Calcular edad automáticamente si viene fecha de nacimiento
                if (dto.FechaNacimiento.HasValue && dto.FechaNacimiento.Value != default)
                {
                    var today = DateTime.Today;
                    var edad = today.Year - dto.FechaNacimiento.Value.Year;

                    // Ajustar si aún no ha cumplido años este año
                    if (dto.FechaNacimiento.Value.Date > today.AddYears(-edad))
                    {
                        edad--;
                    }

                    persona.Edad = edad >= 0 ? edad : null;
                }
                else
                {
                    persona.Edad = null;
                }

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
                    .Include(p => p.Direccion)
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
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Error al crear la persona.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        // ACTUALIZAR
        [HttpPut("Actualizar/{id:guid}")]
        [EndpointSummary("Actualiza una persona existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Update(Guid id, DetallePersonaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();
            try
            {
                var persona = await _context.Personas
                .Include(p => p.Direccion) // incluimos la dirección
                .FirstOrDefaultAsync(p => p.Id == id);

            if (persona == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Persona no encontrada." };
                return NotFound(respuesta);
            }

            // Guardar el UsuarioId original para no modificarlo
            var usuarioIdOriginal = persona.UsuarioId;

            // Actualizamos solo los campos permitidos (NO el UsuarioId ni el Email)
            persona.Nombre = dto.Nombre;
            persona.PrimerApellido = dto.PrimerApellido;
            persona.SegundoApellido = dto.SegundoApellido;
            persona.Genero = dto.Genero;
            persona.FechaNacimiento = dto.FechaNacimiento;
            persona.NumeroIdentificacion = dto.NumeroIdentificacion;
            persona.Imagen = dto.Imagen;
            persona.TipoUsuarioId = dto.TipoUsuarioId;

            // Asegurar que el UsuarioId no cambie
            persona.UsuarioId = usuarioIdOriginal;

            // Recalcular edad automáticamente si viene fecha de nacimiento
            if (dto.FechaNacimiento.HasValue && dto.FechaNacimiento.Value != default)
            {
                var today = DateTime.Today;
                var edad = today.Year - dto.FechaNacimiento.Value.Year;

                if (dto.FechaNacimiento.Value.Date > today.AddYears(-edad))
                {
                    edad--;
                }

                persona.Edad = edad >= 0 ? edad : null;
            }
            else
            {
                persona.Edad = null;
            }

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

            // Reconsultar para incluir todos los datos relacionados
            var actualizada = await _context.Personas
                .Include(p => p.Usuario) // IMPORTANTE: Incluir Usuario para obtener el Email
                .Include(p => p.TipoUsuario)
                .Include(p => p.Direccion)
                .AsNoTracking()
                .FirstAsync(p => p.Id == id);

            // El Email se llenará automáticamente desde Usuario.Email mediante AutoMapper
            respuesta.Status = true;
            respuesta.Response = _mapper.Map<DetallePersonaDTO>(actualizada);
            respuesta.Message = new() { "Persona actualizada correctamente." };
            return Ok(respuesta);
        }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new () { "Error al actualizar la persona.", ex.Message
    };
                return StatusCode(500, respuesta);
}
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
