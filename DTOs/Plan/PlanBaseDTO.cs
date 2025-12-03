namespace VeTLink.DTOs.Plan
{
    public class PlanBaseDTO
    {
        public string NombrePlan { get; set; } = null!;
        public string? Descripcion { get; set; }
        public double Precio { get; set; }
    }
}
