namespace VeTLink.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string NombrePlan { get; set; } = null!;
        public string? Descripcion { get; set; }        
        public double Precio { get; set; }
        public ICollection<Modulo> Modulos { get; set; } = new List<Modulo>();
    }
}
