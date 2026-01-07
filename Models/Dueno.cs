namespace VeTLink.Models
{
    public class Dueno
    {
        public Guid Id { get; set; }

        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; } = new Persona();

        public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}
