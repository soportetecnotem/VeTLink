namespace VeTLink.DTOs
{
    public class UpdateSucursalDTO : SucursalDTO
    {
        public int Id { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
