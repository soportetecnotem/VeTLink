using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeTLink.Data;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialReproductivoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistorialReproductivoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================
        // NUEVO HISTORIAL REPRODUCTIVO
        // ============================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] HistorialReproductivo historial)
        {
            historial.Id = Guid.NewGuid();
            historial.FechaRegistro = DateTime.UtcNow;
            historial.UltimaActualizacion = DateTime.UtcNow;

            _context.HistorialesReproductivos.Add(historial);
            await _context.SaveChangesAsync();

            return Ok(historial);
        }

        // ============================
        // ACTUALIZAR HISTORIAL
        // ============================
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(Guid id, [FromBody] HistorialReproductivo historial)
        {
            if (id != historial.Id)
                return BadRequest("El Id no coincide.");

            var existente = await _context.HistorialesReproductivos
                .FirstOrDefaultAsync(h => h.Id == id);

            if (existente == null)
                return NotFound();

            existente.Esterilizado = historial.Esterilizado;
            existente.FechaEsterilizacion = historial.FechaEsterilizacion;
            existente.Partos = historial.Partos;
            existente.Montas = historial.Montas;
            existente.UltimoCelo = historial.UltimoCelo;
            existente.PrimeraGesta = historial.PrimeraGesta;
            existente.UltimaGesta = historial.UltimaGesta;
            existente.Observaciones = historial.Observaciones;
            existente.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(existente);
        }

        // ============================
        // DETALLES POR ID
        // ============================
        [HttpGet("{id}")]
        public async Task<IActionResult> Detalle(Guid id)
        {
            var historial = await _context.HistorialesReproductivos
                .FirstOrDefaultAsync(h => h.Id == id);

            if (historial == null)
                return NotFound();

            return Ok(historial);
        }

        // ============================
        // LISTADO GENERAL
        // ============================
        [HttpGet]
        public async Task<IActionResult> Listado()
        {
            var historiales = await _context.HistorialesReproductivos
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();

            return Ok(historiales);
        }

        // ============================
        // ELIMINAR HISTORIAL REPRODUCTIVO
        // (solo si no tiene relaciones)
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var historial = await _context.HistorialesReproductivos
                .FirstOrDefaultAsync(h => h.Id == id);

            if (historial == null)
                return NotFound();

            _context.HistorialesReproductivos.Remove(historial);
            await _context.SaveChangesAsync();

            return Ok("Historial reproductivo eliminado correctamente.");
        }
    }
}
