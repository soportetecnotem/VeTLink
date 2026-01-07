using System.ComponentModel.DataAnnotations;
using VeTLink.DTOs.Diagnostico;
using VeTLink.DTOs.Dueno;
using VeTLink.DTOs.Exploracion;
using VeTLink.DTOs.Mascota;
using VeTLink.Models;

namespace VeTLink.DTOs.Consulta
{
    public class ConsultaMedicaDTO
    {
        public int ClinicaId { get; set; }
        // Datos de la mascota (requerido)
        public Guid? MascotaId { get; set; }
        public MascotaConsultaDTO DatosMascota { get; set; } = new MascotaConsultaDTO();

        // Datos del dueño (requerido)
        public Guid? DuenoId { get; set; }
        public DuenoDTO DatosDueno { get; set; } = new DuenoDTO();

        public DateTime? FechaConsulta { get; set; }

        public DateTime? InicioSintomas { get; set; }

        public ExploracionDTO Exploracion { get; set; } = new ExploracionDTO();

        public DiagnosticoDTO Diagnostico { get; set; } = new DiagnosticoDTO();

        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo.")]
        public decimal? Costo { get; set; }

        [StringLength(500, ErrorMessage = "El motivo de consulta no puede exceder los 500 caracteres.")]
        public string? MotivoConsulta { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder los 1000 caracteres.")]
        public string? Observaciones { get; set; }

        public int? TipoServicioId { get; set; }
    }

}