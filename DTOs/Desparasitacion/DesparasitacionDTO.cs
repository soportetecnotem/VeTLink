namespace VeTLink.DTOs.Desparasitacion
{
    public class DesparasitacionDTO
    {
        public Guid Id { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public string? NombreMedicamento { get; set; }
        public DateTime ProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
