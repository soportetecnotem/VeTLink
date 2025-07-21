namespace VeTLink.Models
{
    public class EvidenciaEvolucion
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? URLArchivo { get; set; }
        public string? Descripcion { get; set; }

        public Guid PersonaId { get; set; }
        public Persona? Persona { get; set; }
    }
}
