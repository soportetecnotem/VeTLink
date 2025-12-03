using VeTLink.DTOs.Sucursal;

namespace VeTLink.DTOs.Clinica
{
    public class UpdateClinica : ClinicaDTO
    {
        public ICollection<DetalleSucursalDTO> Sucursales { get; set; } = new List<DetalleSucursalDTO>();

    }
}
