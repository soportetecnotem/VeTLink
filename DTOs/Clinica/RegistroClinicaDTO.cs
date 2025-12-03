using VeTLink.DTOs.Direccion;

namespace VeTLink.DTOs.Clinica
{
    public class RegistroClinicaDTO
    {
        // Datos de clínica
        public string NombreClinica { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? SitioWeb { get; set; }
        public string? EmailClinica { get; set; }
        public bool Activo {  get; set; }
        public int? SuscripcionId { get; set; }

        // Dirección
        public DireccionDTO? Direccion { get; set; }

        // Usuario y persona
        public string Nombre { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int? TipoUsuarioId { get; set; }

        // Veterinario
        public bool EsVeterinario { get; set; }
        public string? CedulaProfesional { get; set; }
        public string? Horarios { get; set; }
    }
}
