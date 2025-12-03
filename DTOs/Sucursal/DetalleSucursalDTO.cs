using VeTLink.DTOs.Direccion;

namespace VeTLink.DTOs.Sucursal
{
    public class DetalleSucursalDTO : SucursalDTO
    {
        public int Id { get; set; }
        public int? ClinicaId { get; set; }
        public string? NombreClinica { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
