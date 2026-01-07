using VeTLink.DTOs.Bano;
using VeTLink.DTOs.Desparasitacion;
using VeTLink.DTOs.Profilaxis;
using VeTLink.DTOs.Vacuna;

namespace VeTLink.DTOs.Carnet
{
    public class CarnetDTO
    {
        public Guid Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Observaciones { get; set; }

        // Registros preventivos
        public List<VacunaDTO> RegistroVacunas { get; set; } = new();
        public List<DesparasitacionDTO> RegistroDesparasitaciones { get; set; } = new();
        public List<BanoDTO> RegistroBanos { get; set; } = new();
        public List<ProfilaxisDTO> RegistroProfilaxis { get; set; } = new();
    }
}
