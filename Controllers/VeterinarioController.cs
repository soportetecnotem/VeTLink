using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VeTLink.Data;
using VeTLink.DTOs;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeterinarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VeterinarioController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Nuevo")]
        [Authorize]
        [EndpointSummary("Crea un nuevo veterinario")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Crear(CreateVeterinarioDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            // 1️ Obtener Id del usuario autenticado (desde el token)
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No se pudo identificar al usuario que realiza la petición." };
                return Unauthorized(respuesta);
            }

            // 2️ Buscar persona del usuario autenticado
            var personaAdmin = await _context.Personas
                .Include(p => p.TipoUsuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (personaAdmin == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No se encontró la persona asociada al usuario actual." };
                return Unauthorized(respuesta);
            }

            // 3️ Validar que la sucursal exista
            var sucursal = await _context.Sucursales
                .Include(s => s.Clinica)
                .FirstOrDefaultAsync(s => s.Id == dto.SucursalId);

            if (sucursal == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Sucursal no encontrada." };
                return NotFound(respuesta);
            }

            // 4️ Si el usuario autenticado es AdminClinica, validar que la sucursal pertenezca a su clínica
            if (personaAdmin.TipoUsuario?.Nombre?.Equals("AdminClinica", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (personaAdmin.ClinicaId == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "El administrador no tiene una clínica asignada." };
                    return BadRequest(respuesta);
                }

                if (sucursal.ClinicaId != personaAdmin.ClinicaId)
                {
                    respuesta.Status = false;
                    respuesta.Message = new()
            {
                "Un AdminClinica solo puede registrar veterinarios en sucursales que pertenezcan a su propia clínica."
            };
                    return BadRequest(respuesta);
                }
            }

            // 5️ Mapear la persona desde el DTO
            var personaVeterinario = _mapper.Map<Persona>(dto.Persona);
            personaVeterinario.TipoUsuarioId = await _context.TiposUsuarios
                .Where(t => t.Nombre == "Veterinario")
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            if (personaVeterinario.TipoUsuarioId == 0)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No se encontró el tipo de usuario 'Veterinario'." };
                return BadRequest(respuesta);
            }

            // 6️ Crear el veterinario con AutoMapper
            var veterinario = _mapper.Map<Veterinario>(dto);
            veterinario.Persona = personaVeterinario;

            // Asociar la sucursal existente
            veterinario.SucursalesAsignadas = new List<Sucursal> { sucursal };

            _context.Veterinarios.Add(veterinario);
            await _context.SaveChangesAsync();

            // 7️ Preparar respuesta
            var detalle = _mapper.Map<DetalleVeterinarioDTO>(veterinario);
            respuesta.Status = true;
            respuesta.Response = detalle;
            respuesta.Message = new() { "Veterinario creado correctamente." };

            return Ok(respuesta);
        }

        [HttpPut("Actualizar/{id:guid}")]
        [Authorize]
        [EndpointSummary("Actualiza un veterinario existente con su persona y sucursal")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Editar(Guid id, UpdateVeterinarioDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            // 1️ Buscar el veterinario con sus relaciones
            var veterinario = await _context.Veterinarios
                .Include(v => v.Persona)
                    .ThenInclude(p => p.Direccion)
                .Include(v => v.SucursalesAsignadas)
                    .ThenInclude(s => s.Clinica)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veterinario == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Veterinario no encontrado." };
                return NotFound(respuesta);
            }

            // 2️ Obtener el usuario que realiza la acción
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No se pudo identificar al usuario autenticado." };
                return Unauthorized(respuesta);
            }

            var personaAdmin = await _context.Personas
                .Include(p => p.TipoUsuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == userId);

            if (personaAdmin == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No se encontró la persona asociada al usuario actual." };
                return Unauthorized(respuesta);
            }

            // 3️ Validar que la sucursal exista (si se envía)
            Sucursal? sucursal = null;
            if (dto.SucursalId.HasValue)
            {
                sucursal = await _context.Sucursales
                    .Include(s => s.Clinica)
                    .FirstOrDefaultAsync(s => s.Id == dto.SucursalId.Value);

                if (sucursal == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Sucursal no encontrada." };
                    return NotFound(respuesta);
                }

                // Validación si es AdminClinica
                if (personaAdmin.TipoUsuario?.Nombre?.Equals("AdminClinica", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (personaAdmin.ClinicaId == null)
                    {
                        respuesta.Status = false;
                        respuesta.Message = new() { "El administrador no tiene una clínica asignada." };
                        return BadRequest(respuesta);
                    }

                    if (sucursal.ClinicaId != personaAdmin.ClinicaId)
                    {
                        respuesta.Status = false;
                        respuesta.Message = new()
                {
                    "Un AdminClinica solo puede editar veterinarios asignados a sucursales de su propia clínica."
                };
                        return BadRequest(respuesta);
                    }
                }
            }

            // 4️ Actualizar datos del veterinario
            veterinario.CedulaProfesional = dto.CedulaProfesional;
            veterinario.Horarios = dto.Horarios;

            // 5️ Actualizar datos de persona (AutoMapper lo hace sin sobrescribir objetos anidados como dirección)
            _mapper.Map(dto.Persona, veterinario.Persona);

            // 6️ Actualizar sucursales asignadas
            if (sucursal != null)
            {
                var sucursalAsignada = veterinario.SucursalesAsignadas.FirstOrDefault(s => s.Id == sucursal.Id);
                if (sucursalAsignada == null)
                {
                    veterinario.SucursalesAsignadas.Clear();
                    veterinario.SucursalesAsignadas.Add(sucursal);
                }
            }

            await _context.SaveChangesAsync();

            // 7️ Generar respuesta
            var detalle = _mapper.Map<DetalleVeterinarioDTO>(veterinario);
            respuesta.Status = true;
            respuesta.Response = detalle;
            respuesta.Message = new() { "Veterinario actualizado correctamente." };

            return Ok(respuesta);
        }

        // DETALLES
        [HttpGet("Detalles/{id:guid}")]
        [EndpointSummary("Obtiene los detalles de un veterinario")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Detalles(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            //var veterinario = await _context.Veterinarios
            //    .Include(v => v.Persona).ThenInclude(p => p.Direccion)
            //    .Include(v => v.ClinicasAsignadas)
            //    .FirstOrDefaultAsync(v => v.Id == id);

            //if (veterinario == null)
            //{
            //    respuesta.Status = false;
            //    respuesta.Message = new() { "Veterinario no encontrado." };
            //    return NotFound(respuesta);
            //}

            //var detalle = _mapper.Map<DetalleVeterinarioDTO>(veterinario);
            //respuesta.Status = true;
            //respuesta.Response = detalle;
            return Ok(respuesta);
        }

        // GET: api/veterinario
        [HttpGet("Listado")]
        [EndpointSummary("Obtiene todos los veterinarios")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetAll()
        {
            var respuesta = new RespuestaObjetoDTO();

            //var veterinarios = await _context.Veterinarios
            //    .Include(v => v.Persona).ThenInclude(p => p.Direccion)
            //    .Include(v => v.Persona).ThenInclude(p => p.TipoUsuario)
            //    .Include(v => v.ClinicasAsignadas)
            //    .AsNoTracking()
            //    .ToListAsync();

            //var listado = _mapper.Map<List<DetalleVeterinarioDTO>>(veterinarios);

            //respuesta.Status = true;
            //respuesta.Response = listado;
            //respuesta.Message = new() { "Listado de veterinarios obtenido correctamente." };
            return Ok(respuesta);
        }

        // GET: api/veterinario/clinica/5
        [HttpGet("Clinica/{clinicaId:int}")]
        [EndpointSummary("Obtiene todos los veterinarios de una clínica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> GetByClinica(int clinicaId)
        {
            var respuesta = new RespuestaObjetoDTO();

            //var clinica = await _context.Clinicas
            //    //.Include(c => c.sucu)
            //    .FirstOrDefaultAsync(c => c.Id == clinicaId);

            //if (clinica == null)
            //{
            //    respuesta.Status = false;
            //    respuesta.Message = new() { "Clínica no encontrada." };
            //    return NotFound(respuesta);
            //}

            //var veterinarios = await _context.Veterinarios
            //    .Include(v => v.Persona).ThenInclude(p => p.Direccion)
            //    .Include(v => v.Persona).ThenInclude(p => p.TipoUsuario)
            //    .Include(v => v.ClinicasAsignadas)
            //    .Where(v => v.ClinicasAsignadas.Any(c => c.Id == clinicaId))
            //    .AsNoTracking()
            //    .ToListAsync();

            //var listado = _mapper.Map<List<DetalleVeterinarioDTO>>(veterinarios);

            //respuesta.Status = true;
            //respuesta.Response = listado;
            //respuesta.Message = new() { $"Listado de veterinarios de la clínica {clinica.NombreClinica} obtenido correctamente." };
            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Eliminar/{id:guid}")]
        [EndpointSummary("Elimina un veterinario, su persona y dirección")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Eliminar(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            //var veterinario = await _context.Veterinarios
            //    .Include(v => v.Persona).ThenInclude(p => p.Direccion)
            //    .FirstOrDefaultAsync(v => v.Id == id);

            //if (veterinario == null)
            //{
            //    respuesta.Status = false;
            //    respuesta.Message = new() { "Veterinario no encontrado." };
            //    return NotFound(respuesta);
            //}

            //if (veterinario.Persona?.Direccion != null)
            //{
            //    _context.Direcciones.Remove(veterinario.Persona.Direccion);
            //}

            //if (veterinario.Persona != null)
            //{
            //    _context.Personas.Remove(veterinario.Persona);
            //}

            //_context.Veterinarios.Remove(veterinario);
            //await _context.SaveChangesAsync();

            //respuesta.Status = true;
            //respuesta.Message = new() { "Veterinario eliminado correctamente junto con su persona y dirección." };
            return Ok(respuesta);
        }
    }
}
