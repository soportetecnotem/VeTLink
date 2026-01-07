namespace VeTLink.DTOs.HistorialReproductivo
{
    /// <summary>
    /// DTO para actualizar el historial reproductivo completo
    /// </summary>
    public class ActualizarHistorialReproductivoDTO
    {
        public bool? Esterilizado { get; set; }
        public DateTime? FechaEsterilizacion { get; set; }
        public int? Partos { get; set; }
        public int? Montas { get; set; }
        public DateTime? UltimoCelo { get; set; }
        public DateTime? PrimeraGesta { get; set; }
        public DateTime? UltimaGesta { get; set; }
        public string? Observaciones { get; set; }
    }
}