namespace VeTLink.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string? NombreSucursal { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? FotoSucursal { get; set; }
        public string? Horario { get; set; }
        public bool Activo { get; set; }

        public int? DireccionId { get; set; }
        public Direccion? Direccion { get; set; }

        public int? ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
    }
}
