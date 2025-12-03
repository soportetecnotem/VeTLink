namespace VeTLink.DTOs.Plan
{
    public class PlanCrearDTO : PlanBaseDTO
    {
        public List<int>? ModuloIds { get; set; } = new();
    }
}
