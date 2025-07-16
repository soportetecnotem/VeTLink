namespace VeTLink.Models
{
    public class Vacuna
    {
        public Guid Id { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public string? NombreVacuna { get; set; }
        public DateTime ProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }

        public Guid CarnetId { get; set; }
        public CarnetPreventivo? Carnet {  get; set; }
    }
}
