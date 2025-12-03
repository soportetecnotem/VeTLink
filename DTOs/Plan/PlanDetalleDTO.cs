using VeTLink.DTOs.Modulo;

namespace VeTLink.DTOs.Plan
{
    public class PlanDetalleDTO : PlanBaseDTO
    {
        public int Id { get; set; }
        public List<ModuloDTO> Modulos { get; set; } = new();
    }
}
