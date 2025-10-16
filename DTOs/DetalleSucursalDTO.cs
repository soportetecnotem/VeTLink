namespace VeTLink.DTOs
{
    public class DetalleSucursalDTO : BaseSucursalDTO
    {
        public int Id { get; set; }
        public int? ClinicaId { get; set; }
        public string? NombreClinica { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
