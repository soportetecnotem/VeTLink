namespace VeTLink.DTOs
{
    public class PlanCrearDTO : PlanBaseDTO
    {
        public List<int>? ModuloIds { get; set; } = new();
    }
}
