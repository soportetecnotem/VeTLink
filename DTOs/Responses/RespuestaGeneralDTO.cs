namespace VeTLink.DTOs.Responses
{
    public class RespuestaGeneralDTO
    {
        public bool Status { get; set; }
        public List<string>? Message { get; set; } = [];
    }
}
