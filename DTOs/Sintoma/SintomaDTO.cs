using VeTLink.Models;

namespace VeTLink.DTOs.Sintoma
{
    public class SintomaDTO
    {
        public DateTime Fecha { get; set; }
        public string? DescripcionSintomas { get; set; }
        public string? Observaciones { get; set; }
        public int? DuracionCantidad { get; set; }

        public int? UnidadTiempoId { get; set; }
        public string? UnidadTiempoNombre { get; set; }

        public int? ComportamientoId { get; set; }
        public string? ComportamientoNombre { get; set; }
    }
}
