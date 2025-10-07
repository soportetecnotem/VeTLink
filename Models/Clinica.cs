namespace VeTLink.Models
{
    public class Clinica
    {
        public int Id { get; set; }
        public string NombreClinica { get; set; } = null!;        
        public string? SitioWeb { get; set; }        
        public string? Logo { get; set; }
        public bool Activo { get; set; }       

        public int? SuscripcionId { get; set; }
        public Suscripcion? Suscripcion { get; set; }

        public ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    }
}
