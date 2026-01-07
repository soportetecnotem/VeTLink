using VeTLink.DTOs.HistorialMedico;

namespace VeTLink.DTOs.Mascota
{
    public class MascotaConsultaDTO : MascotaDTO
    {
       public HistorialMedicoDTO? HistorialMedico { get; set; } = new HistorialMedicoDTO();  
    }
}
