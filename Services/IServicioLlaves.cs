using VeTLink.Models;

namespace VeTLink.Services
{
    public interface IServicioLlaves
    {
        Task<LlaveAPI> CrearLlave(string usuarioId, TipoLlave tipoLlave);
        string GenerarLlave();
    }
}