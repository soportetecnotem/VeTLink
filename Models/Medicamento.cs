namespace VeTLink.Models
{
    public class Medicamento
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Dosis { get; set; }
        public string? Frecuencia { get; set; }
        public string? Duracion { get; set; }

        public int ViaAdministracionId { get; set; }
        public ViaAdministracion? ViaAdministracion { get; set; }
    }
}
