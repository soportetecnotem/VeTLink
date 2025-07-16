namespace VeTLink.Models
{
    public class PruebaLaboratorio
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Resultado { get; set; }
        public string? ArchivoResultado { get; set; }
        public TipoPrueba? TipoPrueba { get; set; }
    }
}
