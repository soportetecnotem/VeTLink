namespace VeTLink.Models
{
    public class Mascota
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Especie { get; set; }
        public string? Raza { get; set; }
        public string? Color { get; set; }
        public string? Caracteristicas { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string? Imagen { get; set; }

        public Guid? DuenoId { get; set; }
        public Dueno? Dueno { get; set; }

        public int CarnetId { get; set; }
        public CarnetPreventivo? Carnet { get; set; }

        public ICollection<ConsultaMedica> Consultas { get; set; } = new List<ConsultaMedica>();
        
        public Guid HistorialMedicoId { get; set; }
        public HistorialMedico? HistorialMedico { get; set; }
    }
}
