using VeTLink.Data;
using VeTLink.Models;

namespace VeTLink.Services
{
    public class ServicioLlaves : IServicioLlaves
    {
        private readonly ApplicationDbContext context;

        public ServicioLlaves(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<LlaveAPI> CrearLlave(string usuarioId, TipoLlave tipoLlave)
        {
            var llave = GenerarLlave();

            var llaveApi = new LlaveAPI
            {
                Llave = llave,
                Activa = true,
                UsuarioId = usuarioId,
                TipoLlave = tipoLlave
            };

            context.Add(llaveApi);
            await context.SaveChangesAsync();
            return llaveApi;
        }
        public string GenerarLlave() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
