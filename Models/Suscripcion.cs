namespace VeTLink.Models
{
    public class Suscripcion
    {
        public Guid Id { get; set; }
        public bool Renovacion { get; set; }
        public DateTime? Vigencia { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? EstadoSuscripcionId { get; set; }
        public EstadoSuscripcion? EstadoSuscripcion { get; set; }
        public int? PlanId { get; set; }
        public Plan? Plan { get; set; }
        public int? ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }     
    }
}
