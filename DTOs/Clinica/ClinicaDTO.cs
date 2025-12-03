using VeTLink.Models;

namespace VeTLink.DTOs.Clinica
{
    public class ClinicaDTO
    {        
        public string NombreClinica { get; set; } = null!;
        public string? SitioWeb { get; set; }
        public string? Logo { get; set; }
        public bool Activo { get; set; }
        public int? SuscripcionId { get; set; }
    }
}
