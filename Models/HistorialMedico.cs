namespace VeTLink.Models
{
    public class HistorialMedico
    {
        public Guid Id { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? Observaciones { get; set; }

        public Guid HistorialReproductivoId { get; set; }
        public HistorialReproductivo? HistorialReproductivo { get; set; }

        public ICollection<HistorialEnfermedad> Enfermedades { get; set; } = new List<HistorialEnfermedad>();
        
        public ICollection<Alergia> Alergias { get; set; } = new List<Alergia>();
    }
}
