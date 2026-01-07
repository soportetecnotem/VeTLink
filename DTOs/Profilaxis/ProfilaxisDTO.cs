namespace VeTLink.DTOs.Profilaxis
{
    public class ProfilaxisDTO
    {
        public Guid Id { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public DateTime ProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
