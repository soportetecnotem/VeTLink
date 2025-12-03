using VeTLink.DTOs.Sucursal;
using VeTLink.DTOs.Suscripcion;
using VeTLink.Models;

namespace VeTLink.DTOs.Clinica
{
    public class DetalleClinicaDTO: ClinicaDTO
    {
        public int Id { get; set; }
        public SuscripcionDTO? Suscripcion { get; set; }
        public ICollection<DetalleSucursalDTO> Sucursales { get; set; } = new List<DetalleSucursalDTO>();

    }
}
