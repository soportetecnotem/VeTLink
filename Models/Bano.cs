namespace VeTLink.Models
{
    public class Bano
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime ProximoBano { get; set; }
        public string? Observaciones { get; set; }

        public Guid CarnetId { get; set; }
        public CarnetPreventivo? Carnet { get; set; }
    }
}
