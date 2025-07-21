using Microsoft.EntityFrameworkCore;

namespace VeTLink.Models
{
    public class ExploracionFisica
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        [Precision(10, 2)] // precisión total de 18 dígitos, 2 decimales
        public decimal? Peso { get; set; }
        [Precision(10, 2)] // precisión total de 18 dígitos, 2 decimales
        public decimal? TemperaturaCorporal { get; set; }
        public int? FrecuenciaCardiaca { get; set; }
        public int? FrecuenciaRespiratoria { get; set; }
        [Precision(10, 2)] // precisión total de 18 dígitos, 2 decimales
        public decimal LlenadoCapilar { get; set; }
        [Precision(10, 2)] // precisión total de 18 dígitos, 2 decimales
        public decimal? TemperaturaRectal { get; set; }
        public string? Observaciones { get; set; }

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
