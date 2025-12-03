using VeTLink.DTOs.Persona;

namespace VeTLink.DTOs.Veterinario
{
    public class UpdateVeterinarioDTO : VeterinarioDTO
    {
        public int? SucursalId { get; set; }
        public DetallePersonaDTO Persona { get; set; } = null!;
    }
}
