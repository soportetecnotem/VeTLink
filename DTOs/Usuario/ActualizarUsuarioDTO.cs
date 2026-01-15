using System.ComponentModel.DataAnnotations;

namespace VeTLink.DTOs.Usuario
{
    public class ActualizarUsuarioDTO
    {
        // Datos de Identity
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string? Email { get; set; }

        // Datos de Persona
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string? Nombre { get; set; }

        [StringLength(100, ErrorMessage = "El primer apellido no puede exceder los 100 caracteres.")]
        public string? PrimerApellido { get; set; }

        [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder los 100 caracteres.")]
        public string? SegundoApellido { get; set; }

        [StringLength(50, ErrorMessage = "El género no puede exceder los 50 caracteres.")]
        public string? Genero { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public int? NumeroIdentificacion { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        public string? Telefono { get; set; }

        public string? Imagen { get; set; }

        // Tipo de usuario
        public int? TipoUsuarioId { get; set; }

        // Roles
        public List<string>? Roles { get; set; }

        // Cambio de contraseña (opcional)
        public string? ContrasenaActual { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "La nueva contraseña debe tener entre 6 y 100 caracteres.")]
        public string? NuevaContrasena { get; set; }
    }
}
