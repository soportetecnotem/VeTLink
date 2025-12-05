using System.ComponentModel.DataAnnotations;
using VeTLink.DTOs.Dueno;
using VeTLink.DTOs.Mascota;

namespace VeTLink.DTOs.Consulta
{
    public class ConsultaMedicaDTO
    {
        // ID de mascota existente (opcional si se proporciona DatosMascota)
        public Guid? MascotaId { get; set; }

        // Datos para crear nueva mascota (requerido si MascotaId es null)
        public MascotaDTO? DatosMascota { get; set; }

        // Datos del dueño (requerido si se crea nueva mascota)
        public DetalleDuenoDTO? DatosDueno { get; set; }

        public DateTime? FechaConsulta { get; set; }

        public DateTime? InicioSintomas { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo.")]
        public decimal? Costo { get; set; }

        [StringLength(500, ErrorMessage = "El motivo de consulta no puede exceder los 500 caracteres.")]
        public string? MotivoConsulta { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder los 1000 caracteres.")]
        public string? Observaciones { get; set; }

        public int? TipoServicioId { get; set; }
    }

}