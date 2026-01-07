using System.ComponentModel.DataAnnotations;

namespace VeTLink.DTOs.Alergia
{
    public class AgregarAlergiaDTO
    {
        [Required(ErrorMessage = "La sustancia es requerida.")]
        [StringLength(100, ErrorMessage = "La sustancia no puede exceder los 100 caracteres.")]
        public string Sustancia { get; set; } = null!;
    }
}
