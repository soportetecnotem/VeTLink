using VeTLink.DTOs.Mascota;

namespace VeTLink.DTOs.Dueno
{
    public class DetalleDuenoDTO: DuenoDTO
    {
        public Guid Id { get; set; }

        public int CantidadMascotas { get; set; }

        // Información de clínica
        public int? ClinicaId { get; set; }
        public string? NombreClinica { get; set; }

        // Lista de mascotas
        public List<DetalleMascotaDTO>? Mascotas { get; set; }
    }
}