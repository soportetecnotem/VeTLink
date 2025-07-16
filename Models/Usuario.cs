using Microsoft.AspNetCore.Identity;

namespace VeTLink.Models
{
    public class Usuario : IdentityUser
    {
        public string? Imagen { get; set; }

        public TipoUsuario? TipoUsuario { get; set; }
        public int? TipoUsuarioId { get; set; }

        public int? DireccionId { get; set; }
        public Direccion? Direccion { get; set; }


    }
}
