using System.ComponentModel.DataAnnotations;
using VeTLink.DTOs.Direccion;

namespace VeTLink.DTOs.Dueno
{
    public class DuenoDTO
    {
        [Required(ErrorMessage = "El nombre del dueño es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El primer apellido es requerido.")]
        [StringLength(100, ErrorMessage = "El primer apellido no puede exceder los 100 caracteres.")]
        public string PrimerApellido { get; set; } = null!;

        [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder los 100 caracteres.")]
        public string? SegundoApellido { get; set; }

        [StringLength(20, ErrorMessage = "El género no puede exceder los 20 caracteres.")]
        public string? Genero { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public int? Edad { get; set; }

        public int? NumeroIdentificacion { get; set; }

        // Email REQUERIDO para crear cuenta permanente
        [Required(ErrorMessage = "El email es requerido para crear la cuenta del dueño.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = null!;

        // Password NO es requerido, se usa "Abc123.#" por defecto
        public string? Password { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        public string? Telefono { get; set; }

        public string? Imagen { get; set; }

        // Dirección del dueño (opcional)
        public DireccionDTO? Direccion { get; set; }
    }
}
