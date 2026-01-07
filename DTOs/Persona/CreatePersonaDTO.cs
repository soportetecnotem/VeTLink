using VeTLink.DTOs.Direccion;
using VeTLink.Models;

namespace VeTLink.DTOs.Persona
{
    public class CreatePersonaDTO: PersonaDTO
    {
        
        public int? TipoUsuarioId { get; set; }
        public DireccionDTO? Direccion { get; set; }
    }
}
