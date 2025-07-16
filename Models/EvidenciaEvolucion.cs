namespace VeTLink.Models
{
    public class EvidenciaEvolucion
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? URLArchivo { get; set; }
        public string? Descripcion { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
