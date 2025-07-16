namespace VeTLink.Models
{
    public class Veterinario
    {
        public Guid Id { get; set; }
        public string? CedulaProfesional { get; set; }
        public string? Horarios { get; set; }

        public Guid? PersonaId { get; set; }
        public Persona? Persona { get; set; }

        public ICollection<Clinica> ClinicasAsignadas { get; set; } = new List<Clinica>();

    }
}
