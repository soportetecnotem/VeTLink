namespace VeTLink.DTOs
{
    public class DetallePersonaDTO: PersonaDTO
    {
        public Guid Id { get; set; }
        public int? NumeroIdentificacion { get; set; }
        public string? Imagen { get; set; }

        public int? TipoUsuarioId { get; set; }
        public string? TipoUsuarioNombre { get; set; }
    }
}
