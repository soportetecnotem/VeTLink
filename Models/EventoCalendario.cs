namespace VeTLink.Models
{
    public class EventoCalendario
    {
        public Guid Id { get; set; }
        public DateTime? FechaEvento { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public bool Pendiente { get; set; }


        public int TipoServicioId { get; set; }
        public TipoServicio? TipoServicio { get; set; }

        public Guid MascotaId { get; set; }
        public Mascota? Mascota { get; set; }
    }
}
