namespace VeTLink.DTOs.Usuario
{
    public class UsuarioSistemaDTO
    {
        // Identificadores
        public Guid Id { get; set; }
        public string UsuarioId { get; set; } = null!;

        // Datos de Identity
        public string Email { get; set; } = null!;
        public bool EmailConfirmado { get; set; }
        public bool CuentaBloqueada { get; set; }

        // Datos de Persona
        public string Nombre { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Genero { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public int? NumeroIdentificacion { get; set; }
        public string? Telefono { get; set; }
        public string? Imagen { get; set; }

        // Tipo de Usuario
        public int? TipoUsuarioId { get; set; }
        public string? TipoUsuarioNombre { get; set; }

        // Clínica
        public int? ClinicaId { get; set; }
        public string? NombreClinica { get; set; }

        // Roles
        public List<string> Roles { get; set; } = new();
    }
}
