using VeTLink.DTOs.Carnet;
using VeTLink.DTOs.Dueno;
using VeTLink.DTOs.HistorialMedico;

namespace VeTLink.DTOs.Mascota
{
    public class DetalleMascotaDTO: MascotaDTO 
    {
        public Guid Id { get; set; }

        // Datos básicos de la mascota
        public string Nombre { get; set; } = null!;
        public string? Especie { get; set; }
        public string? Raza { get; set; }
        public string? Color { get; set; }
        public string? Caracteristicas { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string? Imagen { get; set; }

        // Relación con el dueño
        public Guid? DuenoId { get; set; }
        public string? NombreDueno { get; set; } // Nombre completo del dueño
        public DetalleDuenoDTO? Dueno { get; set; } // Datos completos del dueño (opcional)

        // Historial médico completo
        public Guid HistorialMedicoId { get; set; }
        public HistorialMedicoDTO? HistorialMedico { get; set; }

        // Carnet preventivo
        public Guid CarnetId { get; set; }
        public CarnetDTO? Carnet { get; set; }

    }
}
