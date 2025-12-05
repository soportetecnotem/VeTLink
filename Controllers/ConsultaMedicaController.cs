using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeTLink.Data;
using VeTLink.DTOs.Consulta;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación JWT
    public class ConsultaMedicaController(ApplicationDbContext context,
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Crea una nueva consulta médica. Solo accesible por veterinarios.
        /// Permite crear mascota y dueño si no existen.
        /// </summary>
        [HttpPost("Nueva")]
        [EndpointSummary("Crea una nueva consulta médica (solo veterinarios)")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearConsulta([FromBody] ConsultaMedicaDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Obtener el usuario autenticado
                var usuario = await _userManager.GetUserAsync(User);
                if (usuario == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Usuario no autenticado." };
                    return Unauthorized(respuesta);
                }

                // 2. Verificar que el usuario sea un veterinario
                var persona = await _context.Personas
                    .Include(p => p.TipoUsuario)
                    .FirstOrDefaultAsync(p => p.UsuarioId == usuario.Id);

                if (persona == null || persona.TipoUsuarioId != 1) // 1 = Veterinario
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Solo los veterinarios pueden crear consultas médicas." };
                    return Forbid();
                }

                // 3. Obtener el veterinario asociado a esta persona
                var veterinario = await _context.Veterinarios
                    .FirstOrDefaultAsync(v => v.PersonaId == persona.Id);

                if (veterinario == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se encontró el perfil de veterinario." };
                    return NotFound(respuesta);
                }

                Guid mascotaId;

                // 4. Validar o crear mascota
                if (dto.MascotaId.HasValue && dto.MascotaId.Value != Guid.Empty)
                {
                    // Intentar obtener mascota existente
                    var mascotaExistente = await _context.Mascotas
                        .Include(m => m.Dueno)
                        .FirstOrDefaultAsync(m => m.Id == dto.MascotaId.Value);

                    if (mascotaExistente == null)
                    {
                        respuesta.Status = false;
                        respuesta.Message = new() { "No se encontró la mascota especificada." };
                        return NotFound(respuesta);
                    }

                    mascotaId = mascotaExistente.Id;
                }
                else
                {
                    // La mascota no existe, validar que vengan los datos para crearla
                    if (dto.DatosMascota == null)
                    {
                        respuesta.Status = false;
                        respuesta.Message = new() { "Debe proporcionar el ID de una mascota existente o los datos para crear una nueva." };
                        return BadRequest(respuesta);
                    }

                    if (string.IsNullOrWhiteSpace(dto.DatosMascota.Nombre))
                    {
                        respuesta.Status = false;
                        respuesta.Message = new() { "El nombre de la mascota es requerido." };
                        return BadRequest(respuesta);
                    }

                    if (dto.DatosDueno == null)
                    {
                        respuesta.Status = false;
                        respuesta.Message = new() { "Debe proporcionar los datos del dueño para crear una nueva mascota." };
                        return BadRequest(respuesta);
                    }

                    // 4.1 Procesar dueño (existente o nuevo)
                    Guid duenoId;

                    if (dto.DatosDueno.Id != Guid.Empty)
                    {
                        // Validar que el dueño existe
                        var duenoExistente = await _context.Duenos.FindAsync(dto.DatosDueno.Id);

                        if (duenoExistente == null)
                        {
                            respuesta.Status = false;
                            respuesta.Message = new() { "No se encontró el dueño especificado." };
                            return NotFound(respuesta);
                        }

                        duenoId = duenoExistente.Id;
                    }
                    else
                    {
                        // Crear nuevo dueño con cuenta permanente
                        
                        // Validar que el email no esté registrado
                        var emailExistente = await _userManager.FindByEmailAsync(dto.DatosDueno.Email);
                        if (emailExistente != null)
                        {
                            respuesta.Status = false;
                            respuesta.Message = new() { "El email ya está registrado en el sistema." };
                            return BadRequest(respuesta);
                        }

                        // Crear dirección si se proporciona
                        int? direccionId = null;
                        if (dto.DatosDueno.Direccion != null)
                        {
                            var direccion = new Direccion
                            {
                                Calle = dto.DatosDueno.Direccion.Calle,
                                NoInt = dto.DatosDueno.Direccion.NoInt,
                                NoExt = dto.DatosDueno.Direccion.NoExt,
                                Colonia = dto.DatosDueno.Direccion.Colonia,
                                Municipio = dto.DatosDueno.Direccion.Municipio,
                                Estado = dto.DatosDueno.Direccion.Estado,
                                CP = dto.DatosDueno.Direccion.CP
                            };

                            _context.Direcciones.Add(direccion);
                            await _context.SaveChangesAsync();
                            direccionId = direccion.Id;
                        }

                        // Crear usuario permanente del dueño
                        var usuarioDueno = new IdentityUser
                        {
                            UserName = dto.DatosDueno.Email,
                            Email = dto.DatosDueno.Email,
                            EmailConfirmed = true // Confirmado por defecto al ser creado por el veterinario
                        };
                        //Contraseña por defecto para todos los dueños
                        //hasta que el dueño inicie sesion, se obligará a cambiarla Abc123.#
                        dto.DatosDueno.Password ="Abc123.#";
                        var resultUsuario = await _userManager.CreateAsync(usuarioDueno, dto.DatosDueno.Password);

                        if (!resultUsuario.Succeeded)
                        {
                            respuesta.Status = false;
                            respuesta.Message = resultUsuario.Errors.Select(e => e.Description).ToList();
                            return BadRequest(respuesta);
                        }

                        // Asignar rol de Dueño
                        if (!await _roleManager.RoleExistsAsync("Dueno"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Dueno"));
                        }
                        await _userManager.AddToRoleAsync(usuarioDueno, "Dueno");

                        // Calcular edad del dueño
                        int? edadDueno = null;
                        if (dto.DatosDueno.FechaNacimiento.HasValue && dto.DatosDueno.FechaNacimiento.Value != default)
                        {
                            var today = DateTime.Today;
                            var edad = today.Year - dto.DatosDueno.FechaNacimiento.Value.Year;

                            if (dto.DatosDueno.FechaNacimiento.Value.Date > today.AddYears(-edad))
                            {
                                edad--;
                            }

                            edadDueno = edad >= 0 ? edad : null;
                        }

                        // Crear persona del dueño
                        var personaDueno = new Persona
                        {
                            Id = Guid.NewGuid(),
                            Nombre = dto.DatosDueno.Nombre,
                            PrimerApellido = dto.DatosDueno.PrimerApellido,
                            SegundoApellido = dto.DatosDueno.SegundoApellido,
                            Genero = dto.DatosDueno.Genero,
                            FechaNacimiento = dto.DatosDueno.FechaNacimiento,
                            Edad = edadDueno,
                            NumeroIdentificacion = dto.DatosDueno.NumeroIdentificacion,
                            Imagen = dto.DatosDueno.Imagen,
                            UsuarioId = usuarioDueno.Id,
                            TipoUsuarioId = 2, // 2 = Dueño
                            DireccionId = direccionId
                        };

                        _context.Personas.Add(personaDueno);
                        await _context.SaveChangesAsync();

                        // Crear el dueño
                        var nuevoDueno = new Dueno
                        {
                            Id = Guid.NewGuid(),
                            PersonaId = personaDueno.Id
                        };

                        _context.Duenos.Add(nuevoDueno);
                        await _context.SaveChangesAsync();

                        duenoId = nuevoDueno.Id;
                    }

                    // 4.2 Crear carnet preventivo
                    var carnet = new CarnetPreventivo
                    {
                        Id = Guid.NewGuid(),
                        FechaCreacion = DateTime.UtcNow,
                        Observaciones = "Carnet creado automáticamente"
                    };

                    _context.Carnet.Add(carnet);
                    await _context.SaveChangesAsync();

                    // 4.3 Crear historial reproductivo
                    var historialReproductivo = new HistorialReproductivo
                    {
                        Id = Guid.NewGuid(),
                        FechaRegistro = DateTime.UtcNow,
                        Partos = 0,
                        Montas = 0
                    };

                    _context.HistorialesReproductivos.Add(historialReproductivo);
                    await _context.SaveChangesAsync();

                    // 4.4 Crear historial médico
                    var historialMedico = new HistorialMedico
                    {
                        Id = Guid.NewGuid(),
                        FechaRegistro = DateTime.UtcNow,
                        HistorialReproductivoId = historialReproductivo.Id
                    };

                    _context.HistorialesMedicos.Add(historialMedico);
                    await _context.SaveChangesAsync();

                    // 4.5 Calcular edad de la mascota
                    int? edadMascota = null;
                    if (dto.DatosMascota.FechaNacimiento.HasValue && dto.DatosMascota.FechaNacimiento.Value != default)
                    {
                        var today = DateTime.Today;
                        var edad = today.Year - dto.DatosMascota.FechaNacimiento.Value.Year;

                        if (dto.DatosMascota.FechaNacimiento.Value.Date > today.AddYears(-edad))
                        {
                            edad--;
                        }

                        edadMascota = edad >= 0 ? edad : null;
                    }

                    // 4.6 Crear la mascota
                    var nuevaMascota = new Mascota
                    {
                        Id = Guid.NewGuid(),
                        Nombre = dto.DatosMascota.Nombre,
                        Especie = dto.DatosMascota.Especie,
                        Raza = dto.DatosMascota.Raza,
                        Color = dto.DatosMascota.Color,
                        Caracteristicas = dto.DatosMascota.Caracteristicas,
                        FechaNacimiento = dto.DatosMascota.FechaNacimiento,
                        Edad = edadMascota,
                        Imagen = dto.DatosMascota.Imagen,
                        DuenoId = duenoId,
                        CarnetId = carnet.Id,
                        HistorialMedicoId = historialMedico.Id
                    };

                    _context.Mascotas.Add(nuevaMascota);
                    await _context.SaveChangesAsync();

                    mascotaId = nuevaMascota.Id;
                }

                // 5. Validar tipo de servicio
                if (dto.TipoServicioId.HasValue)
                {
                    var tipoServicio = await _context.TiposServicios
                        .FindAsync(dto.TipoServicioId.Value);

                    if (tipoServicio == null)
                    {
                        respuesta.Status = false;
                        respuesta.Message = new() { "El tipo de servicio no existe." };
                        return BadRequest(respuesta);
                    }
                }

                // 6. Crear la consulta médica
                var consulta = new ConsultaMedica
                {
                    Id = Guid.NewGuid(),
                    FechaConsulta = dto.FechaConsulta ?? DateTime.UtcNow,
                    InicioSintomas = dto.InicioSintomas,
                    Costo = dto.Costo,
                    MotivoConsulta = dto.MotivoConsulta,
                    Observaciones = dto.Observaciones,
                    MascotaId = mascotaId,
                    VeterinarioId = veterinario.Id,
                    TipoServicioId = dto.TipoServicioId ?? 1
                };

                _context.ConsultasMedicas.Add(consulta);
                await _context.SaveChangesAsync();

                // Commit de la transacción
                await transaction.CommitAsync();

                // 7. Recuperar la consulta con sus relaciones para el response
                var consultaCreada = await _context.ConsultasMedicas
                    .Include(c => c.Mascota)
                        .ThenInclude(m => m.Dueno)
                            .ThenInclude(d => d.Persona)
                    .Include(c => c.Veterinario)
                        .ThenInclude(v => v.Persona)
                    .Include(c => c.TipoServicio)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == consulta.Id);

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<DetallesConsultaMedicaDTO>(consultaCreada);
                respuesta.Message = new() { "Consulta médica creada exitosamente." };

                return CreatedAtAction(nameof(ObtenerConsulta), new { id = consulta.Id }, respuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                respuesta.Status = false;
                respuesta.Message = new() { "Error al crear la consulta médica.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        /// <summary>
        /// Obtiene una consulta médica por su ID
        /// </summary>
        [HttpGet("Detalles/{id}")]
        [EndpointSummary("Obtiene una consulta médica específica")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ObtenerConsulta(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            var consulta = await _context.ConsultasMedicas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                    .ThenInclude(v => v.Persona)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sintomas)
                .Include(c => c.Exploracion)
                .Include(c => c.Diagnostico)
                .Include(c => c.Receta)
                .Include(c => c.Evoluciones)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "No se encontró la consulta médica." };
                return NotFound(respuesta);
            }

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<DetallesConsultaMedicaDTO>(consulta);
            respuesta.Message = new() { "Consulta médica obtenida exitosamente." };

            return Ok(respuesta);
        }

        /// <summary>
        /// Obtiene todas las consultas de una mascota
        /// </summary>
        [HttpGet("Mascota/{mascotaId}")]
        [EndpointSummary("Obtiene todas las consultas de una mascota")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ObtenerConsultasPorMascota(Guid mascotaId)
        {
            var respuesta = new RespuestaObjetoDTO();

            var consultas = await _context.ConsultasMedicas
                .Include(c => c.Veterinario)
                    .ThenInclude(v => v.Persona)
                .Include(c => c.TipoServicio)
                .Where(c => c.MascotaId == mascotaId)
                .OrderByDescending(c => c.FechaConsulta)
                .AsNoTracking()
                .ToListAsync();

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<List<DetallesConsultaMedicaDTO>>(consultas);
            respuesta.Message = new() { $"Se encontraron {consultas.Count} consultas." };

            return Ok(respuesta);
        }

        /// <summary>
        /// Obtiene todas las consultas realizadas por un veterinario
        /// </summary>
        [HttpGet("Veterinario/{veterinarioId}")]
        [EndpointSummary("Obtiene todas las consultas realizadas por un veterinario")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ObtenerConsultasPorVeterinario(Guid veterinarioId)
        {
            var respuesta = new RespuestaObjetoDTO();

            var consultas = await _context.ConsultasMedicas
                .Include(c => c.Mascota)
                .Include(c => c.TipoServicio)
                .Where(c => c.VeterinarioId == veterinarioId)
                .OrderByDescending(c => c.FechaConsulta)
                .AsNoTracking()
                .ToListAsync();

            respuesta.Status = true;
            respuesta.Response = _mapper.Map<List<DetallesConsultaMedicaDTO>>(consultas);
            respuesta.Message = new() { $"Se encontraron {consultas.Count} consultas." };

            return Ok(respuesta);
        }
    }
}