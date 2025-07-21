namespace VeTLink.Models
{
    public class SintomaActual
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? DescripcionSintomas { get; set; }
        public string? Observaciones { get; set; }
        public int? DuracionCantidad { get; set; }

        public int? UnidadTiempoId { get; set; }
        public UnidadTiempo? UnidadTiempo { get; set; }

        public int? ComportamientoId { get; set; }
        public Comportamiento? Comportamiento { get; set;}
        
    }
}
