using VeTLink.Models;

namespace VeTLink.DTOs
{
    public class DetalleClinicaDTO: ClinicaDTO
    {
        public int Id { get; set; }
        public ICollection<UpdateSucursalDTO> Sucursales { get; set; } = new List<UpdateSucursalDTO>();

    }
}
