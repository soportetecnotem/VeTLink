using System.ComponentModel.DataAnnotations;

namespace VeTLink.DTOs.Mascota
{
    public class MascotaDTO
    {
        [Required(ErrorMessage = "El nombre de la mascota es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(50, ErrorMessage = "La especie no puede exceder los 50 caracteres.")]
        public string? Especie { get; set; }

        [StringLength(50, ErrorMessage = "La raza no puede exceder los 50 caracteres.")]
        public string? Raza { get; set; }

        [StringLength(50, ErrorMessage = "El color no puede exceder los 50 caracteres.")]
        public string? Color { get; set; }

        [StringLength(200, ErrorMessage = "Las características no pueden exceder los 200 caracteres.")]
        public string? Caracteristicas { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public int? Edad { get; set; }

        public string? Imagen { get; set; }
    }
}
