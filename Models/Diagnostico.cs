namespace VeTLink.Models
{
    public class Diagnostico
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? DiagnosticoPresuntivo { get; set; }
        public string? DiagnosticoDefinitivo { get; set; }
        public string? Observaciones { get; set; }

        public Guid ConsultaMedicaId { get; set; }
        public ConsultaMedica? ConsultaMedica { get; set; }

        public  ICollection<PruebaLaboratorio> Pruebas { get; set; } = new List<PruebaLaboratorio>();

    }
}
