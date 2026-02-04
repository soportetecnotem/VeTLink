namespace VeTLink.DTOs.CP
{
    public class ValidacionCPDTO
    {
        public bool Valido { get; set; }
        public string CP { get; set; } = null!;
        public string? Estado { get; set; }
        public string? Municipio { get; set; }
        public List<string> Colonias { get; set; } = new();
        public int TotalColonias { get; set; }
    }
}
