namespace VeTLink.DTOs
{
    public class ListadoVeterinarioDTO : VeterinarioDTO 
    {
        public Guid Id { get; set; }
        public string NombreCompleto { get; set; } = null!;

        public List<string> Sucursales { get; set; } = new();
        public List<string> Clinicas { get; set; } = new();
    }
}
