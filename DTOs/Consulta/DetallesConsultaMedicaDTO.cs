using VeTLink.DTOs.Diagnostico;
using VeTLink.DTOs.Dueno;
using VeTLink.DTOs.Exploracion;
using VeTLink.DTOs.Mascota;
using VeTLink.DTOs.Sintoma;

namespace VeTLink.DTOs.Consulta
{
    public class DetallesConsultaMedicaDTO 
    {
        // IDs y datos básicos
        public Guid Id { get; set; }
        public DateTime FechaConsulta { get; set; }
        public DateTime? InicioSintomas { get; set; }
        public decimal? Costo { get; set; }
        public string? MotivoConsulta { get; set; }
        public string? Observaciones { get; set; }

        // Información del veterinario
        public Guid VeterinarioId { get; set; }
        public string? NombreVeterinario { get; set; }
        public string? CedulaVeterinario { get; set; }

        // Información del tipo de servicio
        public int TipoServicioId { get; set; }
        public string? TipoServicioNombre { get; set; }

        // Información de la mascota (con datos completos, no solo input)
        public Guid MascotaId { get; set; }
        public DetalleMascotaDTO? Mascota { get; set; }

        // Información del dueño (con datos completos)
        public DetalleDuenoDTO? Dueno { get; set; }

        // Datos clínicos
        public List<SintomaDTO>? Sintomas { get; set; }
        public ExploracionDTO? Exploracion { get; set; }
        public DiagnosticoDTO? Diagnostico { get; set; }
    }
}