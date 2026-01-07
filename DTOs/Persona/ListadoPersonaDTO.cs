namespace VeTLink.DTOs.Persona
{
    public class ListadoPersonaDTO : PersonaDTO
    {
        public Guid Id { get; set; }
        public string UsuarioId { get; set; } = null!;
        public string? Email { get; set; }
        public int? TipoUsuarioId { get; set; }
        public string? TipoUsuarioNombre { get; set; }
    }
}
