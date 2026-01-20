using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using VeTLink.Data;
using VeTLink.DTOs.Llave;
using VeTLink.Models;
using VeTLink.Services;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LlavesAPIController(ApplicationDbContext context,
        IMapper mapper, IServicioLlaves servicioLlaves) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<LlaveDTO>> Get()
        {
            var usuarioid = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var llaves = await context.LlavesAPI
                .Where(x => x.UsuarioId == usuarioid)
                .ToListAsync();
            return mapper.Map<IEnumerable<LlaveDTO>>(llaves);
        }

        [HttpGet("{id:int}", Name = "ObtenerLlaves")]
        public async Task<ActionResult<LlaveDTO>> Get(int id)
        {
            var usuarioid = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var llave = await context.LlavesAPI
                .FirstOrDefaultAsync(x => x.Id == id);

            if (llave is null)
            {
                return NotFound();
            }

            if (llave.UsuarioId != usuarioid)
            {
                return Forbid();
            }

            return mapper.Map<LlaveDTO>(llave);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LlaveCreacionDTO llaveCreacionDTO)
        {
            var usuarioid = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;

            if (llaveCreacionDTO.TipoLlave == TipoLlave.Gratuita)
            {
                var elUsuarioYaTieneLlaveGratuita = await context.LlavesAPI
                    .AnyAsync(x => x.UsuarioId == usuarioid &&
                                   x.TipoLlave == TipoLlave.Gratuita);

                if (elUsuarioYaTieneLlaveGratuita)
                {
                    ModelState.AddModelError(nameof(llaveCreacionDTO.TipoLlave), "Ya existe una llave gratuita para este usuario.");
                    return ValidationProblem();
                }
            }

            var llaveAPI = await servicioLlaves.CrearLlave(usuarioid, llaveCreacionDTO.TipoLlave);
            var llaveDTO = mapper.Map<LlaveDTO>(llaveAPI);
            return CreatedAtRoute("ObtenerLlaves", new { id = llaveAPI.Id }, llaveDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LlaveActualizacionDTO llaveActualizacionDTO)
        {
            var usuarioid = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var llaveDB = await context.LlavesAPI
                .FirstOrDefaultAsync(x => x.Id == id);

            if (llaveDB is null)
            {
                return NotFound();
            }

            if (usuarioid != llaveDB.UsuarioId)
            {
                return Forbid();
            }

            if (llaveActualizacionDTO.ActualizarLlave)
            {
                llaveDB.Llave = servicioLlaves.GenerarLlave();
            }

            llaveDB.Activa = llaveActualizacionDTO.Activa;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioid = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var llaveDB = await context.LlavesAPI
                .FirstOrDefaultAsync(x => x.Id == id);
            if (llaveDB is null)
            {
                return NotFound();
            }
            if (usuarioid != llaveDB.UsuarioId)
            {
                return Forbid();
            }
            if (llaveDB.TipoLlave == TipoLlave.Gratuita)
            {
                ModelState.AddModelError("LlaveAPI", "No se puede eliminar una llave gratuita.");
                return ValidationProblem();
            }

            context.LlavesAPI.Remove(llaveDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
