namespace VeTLink.Models
{
    public class ExploracionFisica
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? Peso { get; set; }
        public decimal? TemperaturaCorporal { get; set; }
        public int? FrecuenciaCardiaca { get; set; }
        public int? FrecuenciaRespiratoria { get; set; }
        public decimal LlenadoCapilar { get; set; }
        public decimal? TemperaturaRectal { get; set; }
        public string? Observaciones { get; set; }

        public Guid ConsultaMedicaId { get; set; }
        public ConsultaMedica? ConsultaMedica { get; set; }

        public int? CondicionCorporalId { get; set; }
        public CondicionCorporal? CondicionCorporal { get; set; }

        public int? EstadoGeneralId { get; set; }
        public EstadoGeneral? EstadoGeneral { get; set; }

        public int? MucosasId { get; set; }
        public Mucosa? Mucosas { get; set; }
                
        public int? HidratacionId { get; set; }
        public Hidratacion? Hidratacion { get; set; }
        
    }
}
