using VeTLink.DTOs.Direccion;

namespace VeTLink.DTOs.Persona
{
    public class DetallePersonaDTO: PersonaDTO
    {
        public Guid Id { get; set; } 
        public int? TipoUsuarioId { get; set; }
        public string? TipoUsuarioNombre { get; set; }
        public string? Email { get; set; }
        public DetalleDireccionDTO? Direccion { get; set; }
    }
}
