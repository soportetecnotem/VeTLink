namespace VeTLink.Models
{
    public class CarnetPreventivo
    {
        public Guid Id { get; set; }        
        public DateTime FechaCreacion { get; set; }
        public string? Observaciones { get; set; }

        public Guid MascotaId { get; set; }
        public Mascota? Mascota { get; set; }

        public ICollection<Vacuna> RegistroVacunas { get; set; } = new List<Vacuna>();
        public ICollection<Desparasitacion> RegistroDesparasitaciones { get; set; } = new List<Desparasitacion>();
        public ICollection<Bano> RegistroBanos { get; set; } = new List<Bano>();
        public ICollection<Profilaxis> RegistroProfilaxis { get; set; } = new List<Profilaxis>();

    }
}
