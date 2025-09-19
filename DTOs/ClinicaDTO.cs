namespace VeTLink.DTOs
{
    public class ClinicaDTO
    {
        
        public string NombreClinica { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
