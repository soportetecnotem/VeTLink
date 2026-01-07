using VeTLink.DTOs.Alergia;
using VeTLink.DTOs.Enfermedad;
using VeTLink.DTOs.HistorialReproductivo;
using VeTLink.Models;

namespace VeTLink.DTOs.HistorialMedico
{
    public class HistorialMedicoDTO
    {
        public Guid Id { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? Observaciones { get; set; }

        public HistorialReproductivoDTO? HistorialReproductivo { get; set; }

        public ICollection<EnfermedadDTO> Enfermedades { get; set; } = new List<EnfermedadDTO>();

        public ICollection<AlergiaDTO> Alergias { get; set; } = new List<AlergiaDTO>();
    
    }
}
