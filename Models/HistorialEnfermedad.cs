namespace VeTLink.Models
{
    public class HistorialEnfermedad
    {
        public int Id { get; set; }
        public DateTime FechaDiagnostico { get; set; }

        public Guid HistorialMedicoId { get; set; }
        public HistorialMedico? HistorialMedico { get; set; }

        public int EnfermedadId { get; set; }
        public HistorialEnfermedad? Enfermedad { get; set; }
    }
}
