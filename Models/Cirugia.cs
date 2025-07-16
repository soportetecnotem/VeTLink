namespace VeTLink.Models
{
    public class Cirugia
    {
        public int Id { get; set; }
        public DateTime? Fecha { get; set; }

        public int TipoId { get; set; }
        public TipoCirugia? Tipo { get; set; }

        public Guid VeterinarioId { get; set; }
        public Veterinario? Veterinario { get; set; }

        public int HistorialMedicoId { get; set; }
        public HistorialMedico? HistorialMedico { get; set; }
    }
}
