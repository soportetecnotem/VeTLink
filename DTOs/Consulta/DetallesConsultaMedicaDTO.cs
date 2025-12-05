using VeTLink.DTOs.Diagnostico;
using VeTLink.DTOs.Exploracion;
using VeTLink.DTOs.Sintoma;

namespace VeTLink.DTOs.Consulta
{
    public class DetallesConsultaMedicaDTO : ConsultaMedicaDTO
    {
        public Guid Id { get; set; }
        public Guid VeterinarioId { get; set; }
        public string? NombreVeterinario { get; set; }
        public string? CedulaVeterinario { get; set; }
        public string? TipoServicioNombre { get; set; }

        public List<SintomaDTO>? Sintomas { get; set; }
        public ExploracionDTO? Exploracion { get; set; }
        public DiagnosticoDTO? Diagnostico { get; set; }
    }
}