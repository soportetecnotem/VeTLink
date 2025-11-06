namespace VeTLink.DTOs
{
    public class SuscripcionDTO
    {
        public bool Renovacion { get; set; }
        public DateTime? Vigencia { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? EstadoSuscripcionId { get; set; }
        public int? PlanId { get; set; }
    }
}
