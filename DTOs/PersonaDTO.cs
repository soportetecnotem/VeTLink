namespace VeTLink.DTOs
{
    public class PersonaDTO
    {
        public string Nombre { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string? Genero { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Email { get; set; } = null!;
        public int? NumeroIdentificacion { get; set; }
        public string? Imagen { get; set; }
    }
}
