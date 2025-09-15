namespace VeTLink.DTOs
{
    public class CreatePersonaDTO: PersonaDTO
    {
        public string UsuarioId { get; set; } = null!;  // FK a AspNetUsers
        public int? TipoUsuarioId { get; set; }
        public int? DireccionId { get; set; }
    }
}
