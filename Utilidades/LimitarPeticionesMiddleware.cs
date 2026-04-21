using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VeTLink.Data;
using VeTLink.DTOs.Llave;
using VeTLink.Models;

namespace VeTLink.Utilidades
{
    public static class  LimitarPeticionesMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionesMiddleware>();
        }
    }
    public class LimitarPeticionesMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IOptionsMonitor<LimitarPeticionesDTO> optionsMonitorLimitarPeticiones;

        public LimitarPeticionesMiddleware(RequestDelegate next,
            IOptionsMonitor<LimitarPeticionesDTO> optionsMonitorLimitarPeticiones)
        {
            this.next = next;
            this.optionsMonitorLimitarPeticiones = optionsMonitorLimitarPeticiones;
        }

        public async Task InvokeAsync(HttpContext httpcontext, ApplicationDbContext context)
        {
            var endpoint = httpcontext.GetEndpoint();

            if (endpoint == null)
            {
                await next(httpcontext);
                return;
            }

            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            if(actionDescriptor is not null)
            {
                var accionTieneAtributoIgnorarLimitarPeticiones = actionDescriptor.MethodInfo
                    .GetCustomAttributes(typeof(DeshabilitarLimitarPeticionesAttribute), inherit : true).Any();

                var controladorTieneAtributoIgnorarLimitarPeticiones = actionDescriptor.ControllerTypeInfo
                    .GetCustomAttributes(typeof(DeshabilitarLimitarPeticionesAttribute), inherit: true).Any();

                if (accionTieneAtributoIgnorarLimitarPeticiones || controladorTieneAtributoIgnorarLimitarPeticiones)
                {
                    await next(httpcontext);
                    return;
                }
            }

            var limitarPeticionesDTO = optionsMonitorLimitarPeticiones.CurrentValue;

            var llaveStringValues = httpcontext.Request.Headers["X-API-Key"];

            if (llaveStringValues.Count == 0)
            {
                httpcontext.Response.StatusCode = 400;
                await httpcontext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");
                return;
            }

            if (llaveStringValues.Count > 1)
            {
                httpcontext.Response.StatusCode = 400;
                await httpcontext.Response.WriteAsync("Solo una llave debe estar presente");
                return;
            }

            var llave = llaveStringValues[0];

            var llaveDB = context.LlavesAPI.FirstOrDefault(x => x.Llave == llave);

            if (llaveDB is null)
            {
                httpcontext.Response.StatusCode = 400;
                await httpcontext.Response.WriteAsync("La llave no existe");
                return;
            }

            if(!llaveDB.Activa)
            {
                httpcontext.Response.StatusCode = 400;
                await httpcontext.Response.WriteAsync("La llave no está activa");
                return;
            }

            if (llaveDB.TipoLlave == TipoLlave.Gratuita)
            {
                var hoy = DateTime.UtcNow.Date;                
                var cantidadPeticionesHoy = await context.Peticiones.CountAsync(x => x.LlaveId == llaveDB.Id 
                && x.FechaPeticion >= hoy);
                    
                if (limitarPeticionesDTO.PeticionesPorDiaGratuito <= cantidadPeticionesHoy )
                {
                    httpcontext.Response.StatusCode = 429; //Too many requests demasiadas peticiones
                    await httpcontext.Response.WriteAsync("Se ha excedido el límite de peticiones por dia para esta llave. Si desea obtener mas peticiones actualice su suscripcion a una profesional");
                    return;
                }
                
                var peticion = new Peticion
                {
                    LlaveId = llaveDB.Id,
                    FechaPeticion = DateTime.UtcNow
                };
                context.Add(peticion);
                await context.SaveChangesAsync();

                await next(httpcontext);
            }
        }
    }
}
