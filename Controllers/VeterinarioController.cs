using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VeTLink.Data;
using VeTLink.DTOs.Responses;
using VeTLink.DTOs.Veterinario;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [EndpointSummary("Actualiza un veterinario existente con su persona y sucursal")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Editar(Guid id, UpdateVeterinarioDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            // 1️ Buscar el veterinario con sus relaciones
            var veterinario = await _context.Veterinarios
                .Include(v => v.Persona)
                    .ThenInclude(p => p!.Direccion)
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
        [EndpointSummary("Obtiene los detalles de un veterinario con su persona, dirección y clínicas asignadas")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Detalles(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var veterinario = await _context.Veterinarios
                .Include(v => v.Persona)
                    .ThenInclude(p => p!.Direccion)
                .Include(v => v.SucursalesAsignadas)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veterinario == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Veterinario no encontrado." };
                return NotFound(respuesta);
            }

            var detalle = _mapper.Map<DetalleVeterinarioDTO>(veterinario);

            respuesta.Status = true;
            respuesta.Response = detalle;
            respuesta.Message = new() { "Detalles del veterinario obtenidos correctamente." };

            return Ok(respuesta);
        }

        [HttpGet("Listado")]
        [EndpointSummary("Obtiene el listado completo de veterinarios con su persona, sucursales y clínicas")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Listado()
        {
            var respuesta = new RespuestaObjetoDTO();

            var veterinarios = await _context.Veterinarios
                .Include(v => v.Persona)
                    .ThenInclude(p => p!.Direccion)
                .Include(v => v.SucursalesAsignadas)
                    .ThenInclude(s => s.Clinica)
                .AsNoTracking()
                .ToListAsync();

            if (!veterinarios.Any())
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No hay veterinarios registrados." };
                return NotFound(respuesta);
            }

            var lista = _mapper.Map<List<ListadoVeterinarioDTO>>(veterinarios);

            respuesta.Status = true;
            respuesta.Response = lista;
            respuesta.Message = new() { "Listado de veterinarios obtenido correctamente." };

            return Ok(respuesta);
        }

        [HttpGet("ListadoPorClinica/{clinicaId:int}")]
        [EndpointSummary("Obtiene todos los veterinarios que pertenecen a una clínica específica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ListadoPorClinica(int clinicaId)
        {
            var respuesta = new RespuestaObjetoDTO();

            var veterinarios = await _context.Veterinarios
                .Include(v => v.Persona)
                    .ThenInclude(p => p!.Direccion)
                .Include(v => v.SucursalesAsignadas)
                    .ThenInclude(s => s.Clinica)
                .Where(v => v.SucursalesAsignadas.Any(s => s.ClinicaId == clinicaId))
                .AsNoTracking()
                .ToListAsync();

            if (!veterinarios.Any())
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No hay veterinarios asignados a esta clínica." };
                return NotFound(respuesta);
            }

            var lista = _mapper.Map<List<ListadoVeterinarioDTO>>(veterinarios);

            respuesta.Status = true;
            respuesta.Response = lista;
            respuesta.Message = new() { "Veterinarios de la clínica obtenidos correctamente." };

            return Ok(respuesta);
        }

        // ELIMINAR
        [HttpDelete("Eliminar/{id:guid}")]
        [EndpointSummary("Elimina un veterinario, su persona y dirección, solo si no tiene registros vinculados")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Eliminar(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            // Buscar el veterinario con todas sus relaciones necesarias
            var veterinario = await _context.Veterinarios
                .Include(v => v.Persona)
                    .ThenInclude(p => p!.Direccion)
                .Include(v => v.SucursalesAsignadas)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veterinario == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Veterinario no encontrado." };
                return NotFound(respuesta);
            }

            //  1️ Verificar si está asignado a alguna sucursal
            if (veterinario.SucursalesAsignadas != null && veterinario.SucursalesAsignadas.Any())
            {
                respuesta.Status = false;
                respuesta.Message = new()
        {
            "No se puede eliminar el veterinario porque está asignado a una o más sucursales."
        };
                return BadRequest(respuesta);
            }

            //  2️ (Opcional) Verificar si tiene citas, tratamientos, o historiales
            // Ejemplo si tuvieras tabla CitasVeterinario
            /*
            bool tieneCitas = await _context.Citas.AnyAsync(c => c.VeterinarioId == id);
            if (tieneCitas)
            {
                respuesta.Status = false;
                respuesta.Message = new()
                {
                    "No se puede eliminar el veterinario porque tiene citas registradas."
                };
                return BadRequest(respuesta);
            }
            */

            //  3️ Eliminar registros relacionados (dirección y persona)
            if (veterinario.Persona?.Direccion != null)
                _context.Direcciones.Remove(veterinario.Persona.Direccion);

            if (veterinario.Persona != null)
                _context.Personas.Remove(veterinario.Persona);

            _context.Veterinarios.Remove(veterinario);
            await _context.SaveChangesAsync();

            //  4️ Respuesta final
            respuesta.Status = true;
            respuesta.Message = new() { "Veterinario eliminado correctamente junto con su persona y dirección." };
            return Ok(respuesta);
        }
    }
}
