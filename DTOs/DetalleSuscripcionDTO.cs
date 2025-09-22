namespace VeTLink.DTOs
{
    public class DetalleSuscripcionDTO
    {
        public Guid Id { get; set; }
        public bool Renovacion { get; set; }
        public DateTime? Vigencia { get; set; }
        public DateTime? FechaAlta { get; set; }

        public int? EstadoSuscripcionId { get; set; }
        public string? EstadoSuscripcionNombre { get; set; }

        public int? PlanId { get; set; }
        public string? PlanNombre { get; set; }

        public int? ClinicaId { get; set; }
        public string? NombreClinica { get; set; }

        //Solo de salida, no se guarda en base
        public DateTime? ProximaFechaPago { get; set; }
    }
}
