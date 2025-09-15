namespace VeTLink.DTOs
{
    public class VeterinarioDTO
    {
        public Guid Id { get; set; }
        public string? CedulaProfesional { get; set; }
        public string? Horarios { get; set; }
        public DetallePersonaDTO Persona { get; set; } = null!;
    }
}
