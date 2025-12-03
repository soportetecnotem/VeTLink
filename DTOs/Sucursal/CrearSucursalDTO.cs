using VeTLink.DTOs.Direccion;

namespace VeTLink.DTOs.Sucursal
{
    public class CrearSucursalDTO : SucursalDTO
    {
        public int ClinicaId { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
