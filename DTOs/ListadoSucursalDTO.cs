namespace VeTLink.DTOs
{
    public class ListadoSucursalDTO
    {
        public int Id { get; set; }
        public string? NombreSucursal { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Activo { get; set; }
        public string? NombreClinica { get; set; }
    }
}
