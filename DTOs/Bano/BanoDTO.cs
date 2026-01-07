namespace VeTLink.DTOs.Bano
{
    public class BanoDTO
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime ProximoBano { get; set; }
        public string? Observaciones { get; set; }
    }
}
