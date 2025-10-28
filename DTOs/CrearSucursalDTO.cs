namespace VeTLink.DTOs
{
    public class CrearSucursalDTO : SucursalDTO
    {
        public int ClinicaId { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
