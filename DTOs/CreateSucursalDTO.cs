namespace VeTLink.DTOs
{
    public class CreateSucursalDTO : BaseSucursalDTO
    {
        public int ClinicaId { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
