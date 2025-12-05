using Microsoft.AspNetCore.Identity;

namespace VeTLink.Models
{
    public class Persona
    {
        public Guid Id { get; set; }        
        public string Nombre { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public int? NumeroIdentificacion { get; set; }
        public string? Telefono { get; set; }
        public string? Imagen { get; set; }

        public string UsuarioId { get; set; } = null!;
        public IdentityUser Usuario { get; set; } = null!;

        public TipoUsuario? TipoUsuario { get; set; }
        public int? TipoUsuarioId { get; set; }

        public int? DireccionId { get; set; }
        public Direccion? Direccion { get; set; }

        // Clínicas (solo si aplica)
        public int? ClinicaId { get; set; } // Solo si es admin
        public Clinica? Clinica { get; set; }
    }
}
