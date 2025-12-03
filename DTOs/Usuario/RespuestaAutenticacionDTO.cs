namespace VeTLink.DTOs.Usuario
{
    public class RespuestaAutenticacionDTO
    {
        public string? Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
