using VeTLink.DTOs.Direccion;
using VeTLink.Models;

namespace VeTLink.DTOs.Persona
{
    public class CreatePersonaDTO: PersonaDTO
    {
        public string UsuarioId { get; set; } = null!;  // FK a AspNetUsers
        public int? TipoUsuarioId { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
