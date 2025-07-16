namespace VeTLink.Models
{
    public class ConsultaMedica
    {
        public Guid Id { get; set; }
        public DateTime FechaConsulta { get; set; }
        public DateTime? InicioSintomas { get; set; }        
        public decimal? Costo { get; set; }
        public string? MotivoConsulta { get; set; }        
        public string? Observaciones { get; set; }

        public Guid RecetaId { get; set; }
        public Receta? Receta {  get; set; }

        public Guid MascotaId { get; set; }
        public  Mascota? Mascota { get; set; }

        public Guid VeterinarioId { get; set; }
        public  Usuario? Veterinario { get; set; }

        public int TipoServicioId { get; set; }
        public TipoServicio? TipoServicio { get; set; }

        public Guid ExploracionId { get; set; }
        public ExploracionFisica? Exploracion { get; set; }

        public Guid DiagnosticoId { get; set; }
        public Diagnostico? Diagnostico { get; set; }

        public ICollection<SintomaActual> Sintomas { get; set; } = new List<SintomaActual>();
                
        public  ICollection<EvolucionClinica> Evoluciones { get; set; } = new List<EvolucionClinica>();

    }
}
