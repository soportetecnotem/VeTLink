using VeTLink.DTOs.Persona;

namespace VeTLink.DTOs.Veterinario
{
    public class DetalleVeterinarioDTO : VeterinarioDTO
    {
        public Guid Id { get; set; }
        public DetallePersonaDTO Persona { get; set; } = null!;
        public List<string> Clinicas { get; set; } = new();
    }
}
