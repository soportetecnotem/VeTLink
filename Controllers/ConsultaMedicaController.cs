using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeTLink.Data;
using VeTLink.DTOs.Consulta;
using VeTLink.DTOs.Dueno;
using VeTLink.DTOs.Responses;
using VeTLink.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
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

                // ========================================
                // 4. VALIDAR/CREAR DUEÑO PRIMERO
                // ========================================

                if (dto.DatosDueno == null & dto.DuenoId == Guid.Empty)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Debe proporcionar los datos del dueño." };
                    return BadRequest(respuesta);
                }

                Guid duenoId;
                Dueno dueno;

                // 4.1 Verificar si se proporciona ID de dueño existente
                if (dto.DuenoId != Guid.Empty)
                {
                    // Intentar buscar dueño existente
                    dueno = await _context.Duenos
                        .Include(d => d.Mascotas)
                        .Include(d => d.Persona)
                        .FirstOrDefaultAsync(d => d.Id == dto.DuenoId);

                    if (dueno != null)
                    {
                        // Dueño encontrado, usar el existente
                        duenoId = dueno.Id;
                    }
                    else
                    {
                        // ID proporcionado pero no existe, validar si hay datos para crear
                        if (string.IsNullOrWhiteSpace(dto.DatosDueno.Nombre) ||
                            string.IsNullOrWhiteSpace(dto.DatosDueno.PrimerApellido) ||
                            string.IsNullOrWhiteSpace(dto.DatosDueno.Email))
                        {
                            respuesta.Status = false;
                            respuesta.Message = new() { "El dueño especificado no existe. Proporcione nombre, apellido y email para crear uno nuevo." };
                            return NotFound(respuesta);
                        }

                        // Crear nuevo dueño
                        duenoId = await CrearNuevoDueno(dto.DatosDueno, persona.ClinicaId);

                        // Recargar el dueño con sus relaciones
                        dueno = await _context.Duenos
                            .Include(d => d.Mascotas)
                            .Include(d => d.Persona)
                            .FirstAsync(d => d.Id == duenoId);
                    }
                }
                else
                {
                    // No se proporciona ID, validar que vengan los datos completos
                    if (string.IsNullOrWhiteSpace(dto.DatosDueno.Nombre) ||
                        string.IsNullOrWhiteSpace(dto.DatosDueno.PrimerApellido) ||
                        string.IsNullOrWhiteSpace(dto.DatosDueno.Email))
                    {
                        respuesta.Status = false;
                        respuesta.Message = new() { "Debe proporcionar nombre, apellido y email del dueño." };
                        return BadRequest(respuesta);
                    }

                    // Crear nuevo dueño
                    duenoId = await CrearNuevoDueno(dto.DatosDueno, persona.ClinicaId);

                    // Recargar el dueño con sus relaciones
                    dueno = await _context.Duenos
                        .Include(d => d.Mascotas)
                        .Include(d => d.Persona)
                        .FirstAsync(d => d.Id == duenoId);
                }

                // VALIDAR/CREAR/ACTUALIZAR MASCOTA E HISTORIAL

                if (dto.DatosMascota == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "Debe proporcionar los datos de la mascota." };
                    return BadRequest(respuesta);
                }

                if (string.IsNullOrWhiteSpace(dto.DatosMascota.Nombre))
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "El nombre de la mascota es requerido." };
                    return BadRequest(respuesta);
                }

                Guid mascotaId;
                Guid historialMedicoId;

              // 5.1 Buscar si existe una mascota con el mismo nombre para este dueño
                var nombreMascotaBuscar = dto.DatosMascota.Nombre.ToLower();
                var mascotaExistente = await _context.Mascotas
                    .Include(m => m.HistorialMedico)
                        .ThenInclude(h => h.HistorialReproductivo)
                    .Include(m => m.HistorialMedico)
                        .ThenInclude(h => h.Enfermedades)
                    .Include(m => m.HistorialMedico)
                        .ThenInclude(h => h.Alergias)
                    .FirstOrDefaultAsync(m => m.DuenoId == duenoId &&
                        m.Nombre.ToLower() == nombreMascotaBuscar);

                if (mascotaExistente != null)
                {
                    // ===== MASCOTA EXISTE - ACTUALIZAR =====

                    // Actualizar datos de la mascota
                    mascotaExistente.Especie = dto.DatosMascota.Especie ?? mascotaExistente.Especie;
                    mascotaExistente.Raza = dto.DatosMascota.Raza ?? mascotaExistente.Raza;
                    mascotaExistente.Color = dto.DatosMascota.Color ?? mascotaExistente.Color;
                    mascotaExistente.Caracteristicas = dto.DatosMascota.Caracteristicas ?? mascotaExistente.Caracteristicas;
                    mascotaExistente.FechaNacimiento = dto.DatosMascota.FechaNacimiento ?? mascotaExistente.FechaNacimiento;
                    mascotaExistente.Imagen = dto.DatosMascota.Imagen ?? mascotaExistente.Imagen;

                    // Recalcular edad si se actualizó la fecha de nacimiento
                    if (mascotaExistente.FechaNacimiento.HasValue && mascotaExistente.FechaNacimiento.Value != default)
                    {
                        var today = DateTime.Today;
                        var edad = today.Year - mascotaExistente.FechaNacimiento.Value.Year;

                        if (mascotaExistente.FechaNacimiento.Value.Date > today.AddYears(-edad))
                        {
                            edad--;
                        }

                        mascotaExistente.Edad = edad >= 0 ? edad : null;
                    }

                    _context.Mascotas.Update(mascotaExistente);
                    await _context.SaveChangesAsync();

                    mascotaId = mascotaExistente.Id;
                    historialMedicoId = mascotaExistente.HistorialMedicoId;

                    // ACTUALIZAR HISTORIAL MÉDICO
                    if (dto.DatosMascota.HistorialMedico != null && mascotaExistente.HistorialMedico != null)
                    {
                        var histMed = mascotaExistente.HistorialMedico;
                        histMed.Observaciones = dto.DatosMascota.HistorialMedico.Observaciones ?? histMed.Observaciones;

                        // Actualizar enfermedades (eliminar las anteriores y agregar las nuevas)
                        if (dto.DatosMascota.HistorialMedico.Enfermedades?.Any() == true)
                        {
                            // Eliminar enfermedades existentes
                            var enfermedadesExistentes = _context.HistorialEnfermedades
                                .Where(he => he.HistorialMedicoId == histMed.Id);
                            _context.HistorialEnfermedades.RemoveRange(enfermedadesExistentes);
                            await _context.SaveChangesAsync();

                            // Agregar nuevas enfermedades
                            foreach (var enfermedad in dto.DatosMascota.HistorialMedico.Enfermedades)
                            {
                                _context.HistorialEnfermedades.Add(new HistorialEnfermedad
                                {
                                    HistorialMedicoId = histMed.Id,
                                    EnfermedadId = enfermedad.Id,
                                    FechaDiagnostico = DateTime.UtcNow
                                });
                            }
                        }

                        // Actualizar alergias (limpiar la colección y agregar las nuevas)
                        if (dto.DatosMascota.HistorialMedico.Alergias?.Any() == true)
                        {
                            // Limpiar las alergias existentes (solo la relación, no las entidades Alergia)
                            histMed.Alergias.Clear();
                            await _context.SaveChangesAsync();

                            // Agregar nuevas alergias
                            foreach (var alergiaId in dto.DatosMascota.HistorialMedico.Alergias)
                            {
                                var alergia = await _context.Alergias.FindAsync(alergiaId);
                                if (alergia != null)
                                {
                                    histMed.Alergias.Add(alergia);
                                }
                            }
                        }

                        _context.HistorialesMedicos.Update(histMed);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    // ===== MASCOTA NO EXISTE - CREAR NUEVA CON HISTORIAL =====

                    // Crear carnet preventivo
                    var carnet = new CarnetPreventivo
                    {
                        Id = Guid.NewGuid(),
                        FechaCreacion = DateTime.UtcNow,
                        Observaciones = "Carnet creado automáticamente"
                    };

                    _context.Carnet.Add(carnet);
                    await _context.SaveChangesAsync();

                    // Crear historial reproductivo CON DATOS DEL DTO
                    var historialReproductivo = new HistorialReproductivo
                    {
                        Id = Guid.NewGuid(),
                        FechaRegistro = DateTime.UtcNow,
                        UltimaActualizacion = DateTime.UtcNow,
                        Esterilizado = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.Esterilizado,
                        FechaEsterilizacion = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.FechaEsterilizacion,
                        Partos = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.Partos ?? 0,
                        Montas = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.Montas ?? 0,
                        UltimoCelo = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.UltimoCelo,
                        PrimeraGesta = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.PrimeraGesta,
                        UltimaGesta = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.UltimaGesta,
                        Observaciones = dto.DatosMascota.HistorialMedico?.HistorialReproductivo?.Observaciones
                    };

                    _context.HistorialesReproductivos.Add(historialReproductivo);
                    await _context.SaveChangesAsync();

                    // Crear historial médico CON DATOS DEL DTO
                    var historialMedico = new HistorialMedico
                    {
                        Id = Guid.NewGuid(),
                        FechaRegistro = DateTime.UtcNow,
                        Observaciones = dto.DatosMascota.HistorialMedico?.Observaciones,
                        HistorialReproductivoId = historialReproductivo.Id
                    };

                    _context.HistorialesMedicos.Add(historialMedico);
                    await _context.SaveChangesAsync();

                    historialMedicoId = historialMedico.Id;

                    // Agregar enfermedades si vienen en el DTO
                    if (dto.DatosMascota.HistorialMedico?.Enfermedades?.Any() == true)
                    {
                        foreach (var enfermedad in dto.DatosMascota.HistorialMedico.Enfermedades)
                        {
                            _context.HistorialEnfermedades.Add(new HistorialEnfermedad
                            {
                                HistorialMedicoId = historialMedico.Id,
                                EnfermedadId = enfermedad.Id,
                                FechaDiagnostico = DateTime.UtcNow
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Agregar alergias si vienen en el DTO
                    if (dto.DatosMascota.HistorialMedico?.Alergias?.Any() == true)
                    {
                        foreach (var alergiaDto in dto.DatosMascota.HistorialMedico.Alergias)
                        {
                            // Buscar si la alergia ya existe por sustancia
                            var alergiaExistente = await _context.Alergias
                                .FirstOrDefaultAsync(a => a.Sustancia == alergiaDto.Sustancia);

                            if (alergiaExistente != null)
                            {
                                // Si existe, agregar la relación
                                historialMedico.Alergias.Add(alergiaExistente);
                            }
                            else
                            {
                                // Si no existe, crear nueva alergia
                                var nuevaAlergia = new Alergia
                                {
                                    Sustancia = alergiaDto.Sustancia
                                };
                                _context.Alergias.Add(nuevaAlergia);
                                await _context.SaveChangesAsync(); // Guardar para obtener el ID

                                historialMedico.Alergias.Add(nuevaAlergia);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Calcular edad de la mascota
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

                    // Crear la mascota
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

                //  CREAR EXPLORACIÓN FÍSICA (si viene en el DTO)

                Guid? exploracionId = null;
                if (dto.Exploracion != null)
                {
                    var exploracion = new ExploracionFisica
                    {
                        Id = Guid.NewGuid(),
                        Fecha = dto.Exploracion.Fecha != default ? dto.Exploracion.Fecha : DateTime.UtcNow,
                        Peso = dto.Exploracion.Peso,
                        TemperaturaCorporal = dto.Exploracion.TemperaturaCorporal,
                        FrecuenciaCardiaca = dto.Exploracion.FrecuenciaCardiaca,
                        FrecuenciaRespiratoria = dto.Exploracion.FrecuenciaRespiratoria,
                        LlenadoCapilar = dto.Exploracion.LlenadoCapilar,
                        TemperaturaRectal = dto.Exploracion.TemperaturaRectal,
                        Observaciones = dto.Exploracion.Observaciones,
                        CondicionCorporalId = dto.Exploracion.CondicionCorporalId,
                        EstadoGeneralId = dto.Exploracion.EstadoGeneralId,
                        MucosasId = dto.Exploracion.MucosasId,
                        HidratacionId = dto.Exploracion.HidratacionId
                    };

                    _context.ExploracionesFisicas.Add(exploracion);
                    await _context.SaveChangesAsync();
                    exploracionId = exploracion.Id;
                }

                //  CREAR DIAGNÓSTICO (si viene en el DTO)

                Guid? diagnosticoId = null;
                if (dto.Diagnostico != null)
                {
                    var diagnostico = new Models.Diagnostico
                    {
                        Id = Guid.NewGuid(),
                        Fecha = dto.Diagnostico.Fecha != default ? dto.Diagnostico.Fecha : DateTime.UtcNow,
                        DiagnosticoPresuntivo = dto.Diagnostico.DiagnosticoPresuntivo,
                        DiagnosticoDefinitivo = dto.Diagnostico.DiagnosticoDefinitivo,
                        Observaciones = dto.Diagnostico.Observaciones
                    };

                    _context.Diagnosticos.Add(diagnostico);
                    await _context.SaveChangesAsync();

                    // Agregar pruebas de laboratorio si vienen en el DTO
                    if (dto.Diagnostico.Pruebas?.Any() == true)
                    {
                        foreach (var prueba in dto.Diagnostico.Pruebas)
                        {
                            prueba.Id = Guid.NewGuid(); // Asegurar nuevo ID
                            diagnostico.Pruebas.Add(prueba);
                        }
                        await _context.SaveChangesAsync();
                    }

                    diagnosticoId = diagnostico.Id;
                }

                // VALIDAR TIPO DE SERVICIO

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

                // CREAR LA CONSULTA MÉDICA

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
                    TipoServicioId = dto.TipoServicioId ?? 1,
                    ExploracionId = exploracionId ?? Guid.Empty,
                    DiagnosticoId = diagnosticoId ?? Guid.Empty,
                    Receta = new Receta
                    {
                        Id = Guid.NewGuid(),
                        Fecha = DateTime.UtcNow
                    }
                };

                _context.ConsultasMedicas.Add(consulta);
                await _context.SaveChangesAsync();

                // Commit de la transacción
                await transaction.CommitAsync();

                // Recuperar la consulta con sus relaciones para el response
                var consultaCreada = await _context.ConsultasMedicas
                    .Include(c => c.Mascota)
                        .ThenInclude(m => m.Dueno)
                            .ThenInclude(d => d.Persona)
                    .Include(c => c.Mascota)
                        .ThenInclude(m => m.HistorialMedico)
                            .ThenInclude(h => h.HistorialReproductivo)
                    .Include(c => c.Mascota)
                        .ThenInclude(m => m.HistorialMedico)
                            .ThenInclude(h => h.Enfermedades)
                    .Include(c => c.Mascota)
                        .ThenInclude(m => m.HistorialMedico)
                            .ThenInclude(h => h.Alergias)
                    .Include(c => c.Veterinario)
                        .ThenInclude(v => v.Persona)
                    .Include(c => c.TipoServicio)
                    .Include(c => c.Exploracion) 
                        .ThenInclude(e => e.CondicionCorporal)
                    .Include(c => c.Exploracion)
                        .ThenInclude(e => e.EstadoGeneral)
                    .Include(c => c.Exploracion)
                        .ThenInclude(e => e.Mucosas)
                    .Include(c => c.Exploracion)
                        .ThenInclude(e => e.Hidratacion)
                    .Include(c => c.Diagnostico) 
                        .ThenInclude(d => d.Pruebas)
                            .ThenInclude(p => p.TipoPrueba)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == consulta.Id);

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<DetallesConsultaMedicaDTO>(consultaCreada);
                respuesta.Message = new() { "Consulta médica creada exitosamente con historial completo." };

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
                    .ThenInclude(e => e.CondicionCorporal)
                .Include(c => c.Exploracion)
                    .ThenInclude(e => e.EstadoGeneral)
                .Include(c => c.Exploracion)
                    .ThenInclude(e => e.Mucosas)
                .Include(c => c.Exploracion)
                    .ThenInclude(e => e.Hidratacion)
                .Include(c => c.Diagnostico)
                    .ThenInclude(d => d.Pruebas)
                        .ThenInclude(p => p.TipoPrueba)
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

        /// <summary>
        /// Método auxiliar para crear un nuevo dueño
        /// </summary>
        private async Task<Guid> CrearNuevoDueno(DuenoDTO datosDueno, int? clinicaId)
        {
            // Validar datos mínimos requeridos ANTES de empezar
            if (string.IsNullOrWhiteSpace(datosDueno.Nombre))
            {
                throw new InvalidOperationException("El nombre del dueño es requerido.");
            }

            if (string.IsNullOrWhiteSpace(datosDueno.PrimerApellido))
            {
                throw new InvalidOperationException("El primer apellido del dueño es requerido.");
            }

            if (string.IsNullOrWhiteSpace(datosDueno.Email))
            {
                throw new InvalidOperationException("El email del dueño es requerido.");
            }

            // Validar que el email no esté registrado
            var emailExistente = await _userManager.FindByEmailAsync(datosDueno.Email);
            if (emailExistente != null)
            {
                throw new InvalidOperationException("El email ya está registrado en el sistema.");
            }

            // Crear dirección si se proporciona
            int? direccionId = null;
            if (datosDueno.Direccion != null)
            {
                var direccion = new Direccion
                {
                    Calle = datosDueno.Direccion.Calle,
                    NoInt = datosDueno.Direccion.NoInt,
                    NoExt = datosDueno.Direccion.NoExt,
                    Colonia = datosDueno.Direccion.Colonia,
                    Municipio = datosDueno.Direccion.Municipio,
                    Estado = datosDueno.Direccion.Estado,
                    CP = datosDueno.Direccion.CP
                };

                _context.Direcciones.Add(direccion);
                await _context.SaveChangesAsync();
                direccionId = direccion.Id;
            }

            // Crear usuario permanente del dueño
            var usuarioDueno = new IdentityUser
            {
                UserName = datosDueno.Email,
                Email = datosDueno.Email,
                EmailConfirmed = true
            };

            // Contraseña por defecto
            var passwordDefecto = "Abc123.#";
            var resultUsuario = await _userManager.CreateAsync(usuarioDueno, passwordDefecto);

            if (!resultUsuario.Succeeded)
            {
                var errores = string.Join(", ", resultUsuario.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al crear usuario: {errores}");
            }

            // Calcular edad del dueño
            int? edadDueno = null;
            if (datosDueno.FechaNacimiento.HasValue && datosDueno.FechaNacimiento.Value != default)
            {
                var today = DateTime.Today;
                var edad = today.Year - datosDueno.FechaNacimiento.Value.Year;

                if (datosDueno.FechaNacimiento.Value.Date > today.AddYears(-edad))
                {
                    edad--;
                }

                edadDueno = edad >= 0 ? edad : null;
            }

            // Crear persona del dueño con TODOS los campos requeridos
            var personaDueno = new Persona
            {
                Id = Guid.NewGuid(),
                Nombre = datosDueno.Nombre ?? throw new InvalidOperationException("Nombre no puede ser null"),
                PrimerApellido = datosDueno.PrimerApellido ?? throw new InvalidOperationException("PrimerApellido no puede ser null"),
                SegundoApellido = datosDueno.SegundoApellido,
                Genero = datosDueno.Genero,
                FechaNacimiento = datosDueno.FechaNacimiento,
                Edad = edadDueno,
                NumeroIdentificacion = datosDueno.NumeroIdentificacion,
                Imagen = datosDueno.Imagen,
                Telefono = datosDueno.Telefono ?? "", // Campo requerido
                UsuarioId = usuarioDueno.Id,
                TipoUsuarioId = 2, // 2 = Dueño
                DireccionId = direccionId,
                ClinicaId = clinicaId
            };

            // Crear el dueño
            var nuevoDueno = new Dueno
            {
                Id = Guid.NewGuid(),
                PersonaId = personaDueno.Id,
                Persona = personaDueno
            };

            _context.Duenos.Add(nuevoDueno);
            await _context.SaveChangesAsync();

            return nuevoDueno.Id;
        }
    }
}