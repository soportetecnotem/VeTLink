namespace VeTLink.Models
{
    public class Persona
    {
        public Guid Id { get; set; }        
        public string Nombre { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public int? NumeroIdentificacion { get; set; }

        //public Guid? UsuarioId { get; set; }
        //public Usuario? Usuario { get; set; } = null!;

        public TipoUsuario? TipoUsuario { get; set; }
        public int? TipoUsuarioId { get; set; }

        public int? DireccionId { get; set; }
        public Direccion? Direccion { get; set; }
    }
}
