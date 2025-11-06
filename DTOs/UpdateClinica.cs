namespace VeTLink.DTOs
{
    public class UpdateClinica : ClinicaDTO
    {
        public ICollection<DetalleSucursalDTO> Sucursales { get; set; } = new List<DetalleSucursalDTO>();

    }
}
