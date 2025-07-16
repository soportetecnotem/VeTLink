namespace VeTLink.Models
{
    public class Tratamiento
    {
        public int Id { get; set; }
        public string? Recomendaciones { get; set; }
        public DateTime FechaInicio { get; set; }
        public string? Descripcion { get; set; }

        public int? TipoTratamientoId { get; set; }
        public TipoTratamiento? TipoTratamiento { get; set; }

        public ICollection<Medicamento>? Medicamentos { get; set; }
    }
}
