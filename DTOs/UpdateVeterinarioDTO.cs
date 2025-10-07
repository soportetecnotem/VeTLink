namespace VeTLink.DTOs
{
    public class UpdateVeterinarioDTO : VeterinarioDTO
    {
        public int? SucursalId { get; set; }
        public DetallePersonaDTO Persona { get; set; } = null!;
    }
}
