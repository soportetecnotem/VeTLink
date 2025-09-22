namespace VeTLink.Models
{
    public class Clinica
    {
        public int Id { get; set; }
        public string NombreClinica { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? SitioWeb { get; set; }
        public string? Email { get; set; }
        public bool Activo { get; set; }

        public int? DireccionId { get; set; }
        public Direccion? Direccion { get; set; }

        public int? SuscripcionId { get; set; }
        public Suscripcion? Suscripcion { get; set; }

    }
}
