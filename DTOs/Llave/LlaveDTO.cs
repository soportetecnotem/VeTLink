namespace VeTLink.DTOs.Llave
{
    public class LlaveDTO
    {
        public int Id { get; set; }
        public required string Llave { get; set; }
        public required string TipoLlave { get; set; }
        public bool Activa { get; set; }
    }
}
