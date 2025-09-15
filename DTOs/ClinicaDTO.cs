namespace VeTLink.DTOs
{
    public class ClinicaDTO
    {
        public int Id { get; set; }
        public string NombreClinica { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
