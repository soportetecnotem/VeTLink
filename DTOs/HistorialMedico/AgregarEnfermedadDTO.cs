using System.ComponentModel.DataAnnotations;

namespace VeTLink.DTOs.HistorialMedico
{
    /// <summary>
    /// DTO para agregar una enfermedad al historial médico
    /// </summary>
    public class AgregarEnfermedadDTO
    {
        [Required(ErrorMessage = "El ID de la enfermedad es requerido.")]
        public int EnfermedadId { get; set; }

        public DateTime? FechaDiagnostico { get; set; }
    }
}