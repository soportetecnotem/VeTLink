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
        public async Task<ActionResult<RespuestaObjetoDTO>> Crear(CreateVeterinarioDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            //int? clinicaDelAdmin = null;

            //// Validamos si el usuario es AdminClinica (según el token)
            //var roleClaim = User.FindFirst("Roles")?.Value; // Claim de rol
            //if (roleClaim != null && roleClaim.Equals("AdminClinica", StringComparison.OrdinalIgnoreCase))
            //{
            //    var claimClinica = User.FindFirst("ClinicaId"); // Claim adicional con la clínica asignada
            //    if (claimClinica != null)
            //        clinicaDelAdmin = int.Parse(claimClinica.Value);

            //    if (!dto.ClinicaId.HasValue || dto.ClinicaId != clinicaDelAdmin)
            //    {
            //        respuesta.Status = false;
            //        respuesta.Message = new() { "Un AdminClinica solo puede registrar veterinarios en su propia clínica." };
            //        return BadRequest(respuesta);
            //    }
            //}


            //var persona = _mapper.Map<Persona>(dto.Persona);

            //var veterinario = new Veterinario
            //{
            //    Id = Guid.NewGuid(),
            //    CedulaProfesional = dto.CedulaProfesional,
            //    Horarios = dto.Horarios,
            //    Persona = persona
            //};

            //if (dto.ClinicaId.HasValue)
            //{
            //    var clinica = await _context.Clinicas.FindAsync(dto.ClinicaId.Value);
            //    if (clinica == null)
            //    {
            //        respuesta.Status = false;
            //        respuesta.Message = new() { "Clínica no encontrada." };
            //        return NotFound(respuesta);
            //    }
            //    veterinario.ClinicasAsignadas.Add(clinica);
            //}

            //_context.Veterinarios.Add(veterinario);
            //await _context.SaveChangesAsync();

            //var detalle = _mapper.Map<DetalleVeterinarioDTO>(veterinario);
            //respuesta.Status = true;
            //respuesta.Response = detalle;
            //respuesta.Message = new() { "Veterinario creado correctamente." };
            return Ok(respuesta);
        }

        // ACTUALIZAR
        [HttpPut("Actualizar/{id:guid}")]
        [EndpointSummary("Actualiza un veterinario existente con su persona y clínica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Editar(Guid id, UpdateVeterinarioDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            //var veterinario = await _context.Veterinarios
            //    .Include(v => v.Persona)
            //    .ThenInclude(p => p.Direccion)
            //    .Include(v => v.ClinicasAsignadas)
            //    .FirstOrDefaultAsync(v => v.Id == id);

            //if (veterinario == null)
            //{
            //    respuesta.Status = false;
            //    respuesta.Message = new() { "Veterinario no encontrado." };
            //    return NotFound(respuesta);
            //}

            //// Actualizar veterinario
            //veterinario.CedulaProfesional = dto.CedulaProfesional;
            //veterinario.Horarios = dto.Horarios;

            //// Actualizar persona y dirección
            //_mapper.Map(dto.Persona, veterinario.Persona);

            //// Actualizar clínica (si aplica)
            //if (dto.ClinicaId.HasValue)
            //{
            //    var clinica = await _context.Clinicas.FindAsync(dto.ClinicaId.Value);
            //    if (clinica != null && !veterinario.ClinicasAsignadas.Any(c => c.Id == dto.ClinicaId))
            //    {
            //        veterinario.ClinicasAsignadas.Add(clinica);
            //    }
            //}

            //await _context.SaveChangesAsync();

            //var detalle = _mapper.Map<DetalleVeterinarioDTO>(veterinario);
            //respuesta.Status = true;
            //respuesta.Response = detalle;
            //respuesta.Message = new() { "Veterinario actualizado correctamente." };
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
