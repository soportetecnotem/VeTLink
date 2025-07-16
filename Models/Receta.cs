namespace VeTLink.Models
{
    public class Receta
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Observaciones { get; set; }
        public string? URLArchivo { get; set; }

        public Tratamiento? Tratamiento { get; set; }
    }
}
