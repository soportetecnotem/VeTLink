namespace VeTLink.DTOs.HistorialReproductivo
{
    public class DetallesHistReproDTO: HistorialReproductivoDTO
    {
        public Guid Id { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? UltimaActualizacion { get; set; }
    }
}
