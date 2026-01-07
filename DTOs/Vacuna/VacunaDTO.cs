namespace VeTLink.DTOs.Vacuna
{
    public class VacunaDTO
    {
        public Guid Id { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public string? NombreVacuna { get; set; }
        public DateTime ProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
