namespace VeTLink.DTOs
{
    public class UpdateVeterinarioDTO : VeterinarioDTO
    {
        public int? ClinicaId { get; set; }
        public DetallePersonaDTO Persona { get; set; } = null!;
    }
}
