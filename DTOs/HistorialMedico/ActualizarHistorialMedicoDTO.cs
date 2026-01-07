using VeTLink.DTOs.HistorialReproductivo;
using VeTLink.DTOs.Enfermedad;
using VeTLink.Models;
using VeTLink.DTOs.Alergia;

namespace VeTLink.DTOs.HistorialMedico
{
    /// <summary>
    /// DTO para actualizar un historial médico completo
    /// </summary>
    public class ActualizarHistorialMedicoDTO
    {
        public string? Observaciones { get; set; }
        public ActualizarHistorialReproductivoDTO? HistorialReproductivo { get; set; }
        public ICollection<HistorialEnfermedadDTO>? Enfermedades { get; set; }

        public ICollection<AlergiaDTO>? Alergias { get; set; }
    }
}