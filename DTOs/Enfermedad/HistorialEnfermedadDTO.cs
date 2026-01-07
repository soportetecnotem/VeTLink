namespace VeTLink.DTOs.Enfermedad
{
    public class HistorialEnfermedadDTO
    {
        public DateTime FechaDiagnostico { get; set; }

        public Guid HistorialMedicoId { get; set; }

        public int EnfermedadId { get; set; }
    }
}
