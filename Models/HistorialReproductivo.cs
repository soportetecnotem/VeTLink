namespace VeTLink.Models
{
    public class HistorialReproductivo
    {
        public Guid Id { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? UltimaActualizacion { get; set; }
        public bool? Esterilizado { get; set; }
        public DateTime? FechaEsterilizacion { get; set; }
        public int Partos { get; set; }
        public int Montas { get; set; }
        public DateTime? UltimoCelo { get; set; }
        public DateTime? PrimeraGesta { get; set; }
        public DateTime? UltimaGesta { get; set; }
        public string? Observaciones { get; set; }

    }
}
