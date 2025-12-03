namespace VeTLink.DTOs.Suscripcion
{
    public class UpdateSuscripcionDTO
    {
        public Guid Id { get; set; }
        public bool? Renovacion { get; set; }
        public DateTime? NuevaVigencia { get; set; }
        public int? EstadoSuscripcionId { get; set; } // 1 = Activo, 2 = Suspendido, 3 = Cancelado
    }
}
