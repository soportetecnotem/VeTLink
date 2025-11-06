using VeTLink.Models;

namespace VeTLink.DTOs
{
    public class DetalleClinicaDTO: ClinicaDTO
    {
        public int Id { get; set; }
        public SuscripcionDTO? Suscripcion { get; set; }
        public ICollection<DetalleSucursalDTO> Sucursales { get; set; } = new List<DetalleSucursalDTO>();

    }
}
