using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using VeTLink.Data;
using VeTLink.DTOs.Mascota;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MascotaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MascotaController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================
        // NUEVA MASCOTA
        // ============================
        [HttpPost("Nuevo")]
        [EndpointSummary("Crea una nueva mascota")]
        public async Task<IActionResult> Crear([FromBody] CrearMascotaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que el dueño existe
                var duenoExiste = await _context.Duenos.AnyAsync(d => d.Id == dto.DuenoId);
                if (!duenoExiste)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "El dueño especificado no existe." };
                    return NotFound(respuesta);
                }

                // Crear carnet preventivo
                var carnet = new CarnetPreventivo
                {
                    Id = Guid.NewGuid(),
                    FechaCreacion = DateTime.UtcNow,
                    Observaciones = "Carnet creado automáticamente"
                };
                _context.Carnet.Add(carnet);
                await _context.SaveChangesAsync();

                // Crear historial reproductivo
                var historialReproductivo = new HistorialReproductivo
                {
                    Id = Guid.NewGuid(),
                    FechaRegistro = DateTime.UtcNow,
                    UltimaActualizacion = DateTime.UtcNow,
                    Partos = 0,
                    Montas = 0
                };
                _context.HistorialesReproductivos.Add(historialReproductivo);
                await _context.SaveChangesAsync();

                // Crear historial médico
                var historialMedico = new HistorialMedico
                {
                    Id = Guid.NewGuid(),
                    FechaRegistro = DateTime.UtcNow,
                    HistorialReproductivoId = historialReproductivo.Id
                };
                _context.HistorialesMedicos.Add(historialMedico);
                await _context.SaveChangesAsync();

                // Calcular edad
                int? edad = null;
                if (dto.FechaNacimiento.HasValue && dto.FechaNacimiento.Value != default)
                {
                    var today = DateTime.Today;
                    edad = today.Year - dto.FechaNacimiento.Value.Year;
                    if (dto.FechaNacimiento.Value.Date > today.AddYears(-edad.Value))
                    {
                        edad--;
                    }
                    edad = edad >= 0 ? edad : null;
                }

                // Crear la mascota
                var mascota = new Mascota
                {
                    Id = Guid.NewGuid(),
                    Nombre = dto.Nombre,
                    Especie = dto.Especie,
                    Raza = dto.Raza,
                    Color = dto.Color,
                    Caracteristicas = dto.Caracteristicas,
                    FechaNacimiento = dto.FechaNacimiento,
                    Edad = edad,
                    Imagen = dto.Imagen,
                    DuenoId = dto.DuenoId,
                    CarnetId = carnet.Id,
                    HistorialMedicoId = historialMedico.Id
                };

                _context.Mascotas.Add(mascota);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Recuperar la mascota con sus relaciones
                var mascotaCreada = await _context.Mascotas
                    .Include(m => m.Dueno)
                        .ThenInclude(d => d.Persona)
                    .Include(m => m.HistorialMedico)
                        .ThenInclude(h => h.HistorialReproductivo)
                    .Include(m => m.Carnet)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == mascota.Id);

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<DetalleMascotaDTO>(mascotaCreada);
                respuesta.Message = new() { "Mascota creada exitosamente." };

                return CreatedAtAction(nameof(ObtenerPorId), new { id = mascota.Id }, respuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                respuesta.Status = false;
                respuesta.Message = new() { "Error al crear la mascota.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        // ============================
        // Editar MASCOTA
        // ============================
        [HttpPut("Editar/{id}")]
        [EndpointSummary("Actualiza una mascota existente")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] MascotaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var mascota = await _context.Mascotas.FindAsync(id);
                if (mascota == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Mascota no encontrada." };
                    return NotFound(respuesta);
                }

                // Actualizar solo los campos proporcionados
                mascota.Nombre = dto.Nombre ?? mascota.Nombre;
                mascota.Especie = dto.Especie ?? mascota.Especie;
                mascota.Raza = dto.Raza ?? mascota.Raza;
                mascota.Color = dto.Color ?? mascota.Color;
                mascota.Caracteristicas = dto.Caracteristicas ?? mascota.Caracteristicas;
                mascota.FechaNacimiento = dto.FechaNacimiento ?? mascota.FechaNacimiento;
                mascota.Imagen = dto.Imagen ?? mascota.Imagen;

                // Recalcular edad si cambió la fecha de nacimiento
                if (mascota.FechaNacimiento.HasValue && mascota.FechaNacimiento.Value != default)
                {
                    var today = DateTime.Today;
                    var edad = today.Year - mascota.FechaNacimiento.Value.Year;
                    if (mascota.FechaNacimiento.Value.Date > today.AddYears(-edad))
                    {
                        edad--;
                    }
                    mascota.Edad = edad >= 0 ? edad : null;
                }

                _context.Mascotas.Update(mascota);
                await _context.SaveChangesAsync();

                // Recuperar la mascota actualizada con sus relaciones
                var mascotaActualizada = await _context.Mascotas
                    .Include(m => m.Dueno)
                        .ThenInclude(d => d.Persona)
                    .Include(m => m.HistorialMedico)
                        .ThenInclude(h => h.HistorialReproductivo)
                    .Include(m => m.Carnet)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<DetalleMascotaDTO>(mascotaActualizada);
                respuesta.Message = new() { "Mascota actualizada exitosamente." };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Error al actualizar la mascota.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        // ============================
        // OBTENER POR ID
        // ============================
        /// <summary>
        /// Obtiene los detalles completos de una mascota
        /// </summary>
        [HttpGet("Detalles/{id}")]
        [EndpointSummary("Obtiene una mascota por su ID")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ObtenerPorId(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var mascota = await _context.Mascotas
                .Include(m => m.Dueno)
                    .ThenInclude(d => d.Persona)
                .Include(m => m.HistorialMedico)
                    .ThenInclude(h => h.HistorialReproductivo)
                .Include(m => m.HistorialMedico)
                    .ThenInclude(h => h.Enfermedades)
                        .ThenInclude(e => e.Enfermedad)
                .Include(m => m.HistorialMedico)
                    .ThenInclude(h => h.Alergias)
                .Include(m => m.Carnet)
                    .ThenInclude(c => c.RegistroVacunas)
                .Include(m => m.Carnet)
                    .ThenInclude(c => c.RegistroDesparasitaciones)
                .Include(m => m.Carnet)
                    .ThenInclude(c => c.RegistroBanos)
                .Include(m => m.Carnet)
                    .ThenInclude(c => c.RegistroProfilaxis)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mascota == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Mascota no encontrada." };
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<DetalleMascotaDTO>(mascota);
            respuesta.Message = new() { "Mascota obtenida exitosamente." };

            return Ok(respuesta);
        }

        // ============================
        // LISTADO POR DUEÑO
        // ============================
        [HttpGet("Listado-por-dueno/{duenoId}")]
        [EndpointSummary("Obtiene mascotas por dueño")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ListadoPorDueno(Guid duenoId)
        {
            var respuesta = new RespuestaObjetoDTO();

            // Validar que el dueño existe
            var duenoExiste = await _context.Duenos.AnyAsync(d => d.Id == duenoId);
            if (!duenoExiste)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "El dueño especificado no existe." };
                return NotFound(respuesta);
            }

            var mascotas = await _context.Mascotas
                .Include(m => m.Carnet)
                .Include(m => m.HistorialMedico)
                    .ThenInclude(h => h.HistorialReproductivo)
                .Where(m => m.DuenoId == duenoId)
                .OrderBy(m => m.Nombre)
                .AsNoTracking()
                .ToListAsync();

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<List<MascotaDTO>>(mascotas);
            respuesta.Message = new() { $"Se encontraron {mascotas.Count} mascota(s)." };

            return Ok(respuesta);
        }

        // ============================
        // LISTADO GENERAL PAGINADO
        // ============================
        [HttpGet("ListadoPaginado")]
        [EndpointSummary("Obtiene listado paginado de mascotas")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ListadoPaginado(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? buscar = null)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                var query = _context.Mascotas
                    .Include(m => m.Dueno)
                        .ThenInclude(d => d.Persona)
                    .AsQueryable();

                // Filtro de búsqueda
                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    var buscarLower = buscar.ToLower();
                    query = query.Where(m =>
                        m.Nombre.ToLower().Contains(buscarLower) ||
                        (m.Especie != null && m.Especie.ToLower().Contains(buscarLower)) ||
                        (m.Raza != null && m.Raza.ToLower().Contains(buscarLower)));
                }

                var total = await query.CountAsync();

                var mascotas = await query
                    .OrderBy(m => m.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                var mascotasDTO = _mapper.Map<List<MascotaDTO>>(mascotas);

                respuesta.Status = true;
                respuesta.Response = new
                {
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize),
                    data = mascotasDTO
                };
                respuesta.Message = new() { $"Se encontraron {total} mascota(s)." };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Error al obtener el listado de mascotas.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        // ============================
        // ELIMINAR (SOLO SI NO TIENE RELACIONES)
        // ============================
        [HttpDelete("Eliminar/{id}")]
        [EndpointSummary("Elimina una mascota")]
        public async Task<ActionResult<RespuestaObjetoDTO>> Eliminar(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var mascota = await _context.Mascotas
                    .Include(m => m.Consultas)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (mascota == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Mascota no encontrada." };
                    return NotFound(respuesta);
                }

                // Validar que no tenga consultas médicas
                if (mascota.Consultas.Any())
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se puede eliminar la mascota porque tiene consultas médicas registradas." };
                    return BadRequest(respuesta);
                }

                // Eliminar historial médico y reproductivo
                var historialMedico = await _context.HistorialesMedicos
                    .Include(h => h.HistorialReproductivo)
                    .Include(h => h.Enfermedades)
                    .FirstOrDefaultAsync(h => h.Id == mascota.HistorialMedicoId);

                if (historialMedico != null)
                {
                    // Eliminar enfermedades relacionadas
                    _context.HistorialEnfermedades.RemoveRange(historialMedico.Enfermedades);

                    // Eliminar historial reproductivo
                    if (historialMedico.HistorialReproductivo != null)
                    {
                        _context.HistorialesReproductivos.Remove(historialMedico.HistorialReproductivo);
                    }

                    _context.HistorialesMedicos.Remove(historialMedico);
                }

                // Eliminar carnet preventivo
                var carnet = await _context.Carnet
                    .Include(c => c.RegistroVacunas)
                    .Include(c => c.RegistroDesparasitaciones)
                    .Include(c => c.RegistroBanos)
                    .Include(c => c.RegistroProfilaxis)
                    .FirstOrDefaultAsync(c => c.Id == mascota.CarnetId);

                if (carnet != null)
                {
                    _context.Carnet.Remove(carnet);
                }

                // Eliminar la mascota
                _context.Mascotas.Remove(mascota);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                respuesta.Status = true;
                respuesta.Message = new() { "Mascota eliminada correctamente." };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                respuesta.Status = false;
                respuesta.Message = new() { "Error al eliminar la mascota.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }
    }
}
