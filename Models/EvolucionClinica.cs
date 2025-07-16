namespace VeTLink.Models
{
    public class EvolucionClinica
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? NotasMedico { get; set; }
        public string? Observaciones { get; set; }

        public ICollection<EvidenciaEvolucion> Evidencias { get; set; } = new List<EvidenciaEvolucion>();

    }
}
