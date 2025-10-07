using VeTLink.Models;

namespace VeTLink.DTOs
{
    public class DetalleClinicaDTO: ClinicaDTO
    {
        public int Id { get; set; }
        public ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();

    }
}
