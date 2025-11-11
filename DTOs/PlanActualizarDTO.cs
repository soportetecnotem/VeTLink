namespace VeTLink.DTOs
{
    public class PlanActualizarDTO : PlanBaseDTO
    {
        public int Id { get; set; }
        public List<int>? ModuloIds { get; set; } = new();
    }
}
