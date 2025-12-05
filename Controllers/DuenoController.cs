using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeTLink.Data;
using VeTLink.DTOs.Dueno;
using VeTLink.DTOs.Responses;
using VeTLink.Models;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación JWT
    public class DuenoController(ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Obtiene el ID de la clínica del usuario actual
        /// </summary>
        private async Task<int?> ObtenerClinicaIdDelUsuario()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return null;

            var persona = await _context.Personas
                .FirstOrDefaultAsync(p => p.UsuarioId == usuario.Id);

            return persona?.ClinicaId;
        }

        /// <summary>
        /// Crea un nuevo dueño
        /// </summary>
        [HttpPost("Nuevo")]
        [EndpointSummary("Crea un nuevo dueño con cuenta de usuario")]
        public async Task<ActionResult<RespuestaObjetoDTO>> CrearDueno([FromBody] DuenoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obtener clínica del usuario actual
                var clinicaId = await ObtenerClinicaIdDelUsuario();
                if (clinicaId == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se pudo determinar la clínica del usuario." };
                    return BadRequest(respuesta);
                }

                // Validar que el email no esté registrado
                var emailExistente = await _userManager.FindByEmailAsync(dto.Email);
                if (emailExistente != null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "El email ya está registrado en el sistema." };
                    return BadRequest(respuesta);
                }

                // Crear dirección si se proporciona
                int? direccionId = null;
                if (dto.Direccion != null)
                {
                    var direccion = new Direccion
                    {
                        Calle = dto.Direccion.Calle,
                        NoInt = dto.Direccion.NoInt,
                        NoExt = dto.Direccion.NoExt,
                        Colonia = dto.Direccion.Colonia,
                        Municipio = dto.Direccion.Municipio,
                        Estado = dto.Direccion.Estado,
                        CP = dto.Direccion.CP
                    };

                    _context.Direcciones.Add(direccion);
                    await _context.SaveChangesAsync();
                    direccionId = direccion.Id;
                }

                // Crear usuario del dueño
                var usuarioDueno = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true
                };

                var resultUsuario = await _userManager.CreateAsync(usuarioDueno, dto.Password);

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

                // Calcular edad
                int? edad = null;
                if (dto.FechaNacimiento.HasValue && dto.FechaNacimiento.Value != default)
                {
                    var today = DateTime.Today;
                    var edadCalculada = today.Year - dto.FechaNacimiento.Value.Year;

                    if (dto.FechaNacimiento.Value.Date > today.AddYears(-edadCalculada))
                    {
                        edadCalculada--;
                    }

                    edad = edadCalculada >= 0 ? edadCalculada : null;
                }

                // Crear persona del dueño
                var personaDueno = new Persona
                {
                    Id = Guid.NewGuid(),
                    Nombre = dto.Nombre,
                    PrimerApellido = dto.PrimerApellido,
                    SegundoApellido = dto.SegundoApellido,
                    Genero = dto.Genero,
                    FechaNacimiento = dto.FechaNacimiento,
                    Edad = edad,
                    NumeroIdentificacion = dto.NumeroIdentificacion,
                    Imagen = dto.Imagen,
                    UsuarioId = usuarioDueno.Id,
                    TipoUsuarioId = 2, // 2 = Dueño
                    DireccionId = direccionId,
                    ClinicaId = clinicaId.Value
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

                await transaction.CommitAsync();

                // Obtener el dueño creado con sus relaciones
                var duenoCreado = await _context.Duenos
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Direccion)
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Clinica)
                    .Include(d => d.Mascotas)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == nuevoDueno.Id);

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<DetalleDuenoDTO>(duenoCreado);
                respuesta.Message = new() { "Dueño creado exitosamente." };

                return CreatedAtAction(nameof(ObtenerDetalles), new { id = nuevoDueno.Id }, respuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                respuesta.Status = false;
                respuesta.Message = new() { "Error al crear el dueño.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        /// <summary>
        /// Obtiene los detalles de un dueño específico
        /// </summary>
        [HttpGet("Detalles/{id}")]
        [EndpointSummary("Obtiene los detalles completos de un dueño")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ObtenerDetalles(Guid id)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                // Obtener clínica del usuario actual
                var clinicaId = await ObtenerClinicaIdDelUsuario();
                if (clinicaId == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se pudo determinar la clínica del usuario." };
                    return BadRequest(respuesta);
                }

                var dueno = await _context.Duenos
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Direccion)
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Clinica)
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Usuario)
                    .Include(d => d.Mascotas)
                    .Where(d => d.Persona != null && d.Persona.ClinicaId == clinicaId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dueno == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se encontró el dueño o no pertenece a su clínica." };
                    return NotFound(respuesta);
                }

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<DetalleDuenoDTO>(dueno);
                respuesta.Message = new() { "Dueño obtenido exitosamente." };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Error al obtener el dueño.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        /// <summary>
        /// Actualiza la información de un dueño
        /// </summary>
        [HttpPut("Actualizar/{id:guid}")]
        [EndpointSummary("Actualiza la información de un dueño existente")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ActualizarDueno(Guid id, [FromBody] DetalleDuenoDTO dto)
        {
            var respuesta = new RespuestaObjetoDTO();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que el ID esté presente
                if (id == Guid.Empty)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "El ID del dueño es requerido." };
                    return BadRequest(respuesta);
                }

                // Obtener clínica del usuario actual
                var clinicaId = await ObtenerClinicaIdDelUsuario();
                if (clinicaId == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se pudo determinar la clínica del usuario." };
                    return BadRequest(respuesta);
                }

                var dueno = await _context.Duenos
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Direccion)
                    .Where(d => d.Persona != null && d.Persona.ClinicaId == clinicaId.Value)
                    .FirstOrDefaultAsync(d => d.Id == dto.Id);

                if (dueno == null || dueno.Persona == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se encontró el dueño o no pertenece a su clínica." };
                    return NotFound(respuesta);
                }

                // Actualizar persona
                dueno.Persona.Nombre = dto.Nombre;
                dueno.Persona.PrimerApellido = dto.PrimerApellido;
                dueno.Persona.SegundoApellido = dto.SegundoApellido;
                dueno.Persona.Genero = dto.Genero;
                dueno.Persona.FechaNacimiento = dto.FechaNacimiento;
                dueno.Persona.NumeroIdentificacion = dto.NumeroIdentificacion;
                dueno.Persona.Imagen = dto.Imagen;

                // Calcular edad
                if (dto.FechaNacimiento.HasValue && dto.FechaNacimiento.Value != default)
                {
                    var today = DateTime.Today;
                    var edad = today.Year - dto.FechaNacimiento.Value.Year;

                    if (dto.FechaNacimiento.Value.Date > today.AddYears(-edad))
                    {
                        edad--;
                    }

                    dueno.Persona.Edad = edad >= 0 ? edad : null;
                }

                // Actualizar o crear dirección
                if (dto.Direccion != null)
                {
                    if (dueno.Persona.DireccionId.HasValue && dueno.Persona.Direccion != null)
                    {
                        // Actualizar dirección existente
                        dueno.Persona.Direccion.Calle = dto.Direccion.Calle;
                        dueno.Persona.Direccion.NoInt = dto.Direccion.NoInt;
                        dueno.Persona.Direccion.NoExt = dto.Direccion.NoExt;
                        dueno.Persona.Direccion.Colonia = dto.Direccion.Colonia;
                        dueno.Persona.Direccion.Municipio = dto.Direccion.Municipio;
                        dueno.Persona.Direccion.Estado = dto.Direccion.Estado;
                        dueno.Persona.Direccion.CP = dto.Direccion.CP;
                    }
                    else
                    {
                        // Crear nueva dirección
                        var nuevaDireccion = new Direccion
                        {
                            Calle = dto.Direccion.Calle,
                            NoInt = dto.Direccion.NoInt,
                            NoExt = dto.Direccion.NoExt,
                            Colonia = dto.Direccion.Colonia,
                            Municipio = dto.Direccion.Municipio,
                            Estado = dto.Direccion.Estado,
                            CP = dto.Direccion.CP
                        };

                        _context.Direcciones.Add(nuevaDireccion);
                        await _context.SaveChangesAsync();
                        dueno.Persona.DireccionId = nuevaDireccion.Id;
                    }
                }

                _context.Personas.Update(dueno.Persona);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Obtener el dueño actualizado
                var duenoActualizado = await _context.Duenos
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Direccion)
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Clinica)
                    .Include(d => d.Mascotas)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == id);

                respuesta.Status = true;
                respuesta.Response = _mapper.Map<DetalleDuenoDTO>(duenoActualizado);
                respuesta.Message = new() { "Dueño actualizado exitosamente." };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                respuesta.Status = false;
                respuesta.Message = new() { "Error al actualizar el dueño.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }

        /// <summary>
        /// Lista los dueños de la clínica con filtrado opcional y paginación
        /// </summary>
        [HttpGet("Listado")]
        [EndpointSummary("Obtiene un listado paginado de dueños con filtrado opcional por nombre")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ObtenerListado(
            [FromQuery] string? nombre = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var respuesta = new RespuestaObjetoDTO();

            try
            {
                // Validar parámetros de paginación
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Límite máximo

                // Obtener clínica del usuario actual
                var clinicaId = await ObtenerClinicaIdDelUsuario();
                if (clinicaId == null)
                {
                    respuesta.Status = false;
                    respuesta.Message = new() { "No se pudo determinar la clínica del usuario." };
                    return BadRequest(respuesta);
                }

                // Query base - solo dueños de la clínica actual
                var query = _context.Duenos
                    .Include(d => d.Persona)
                        .ThenInclude(p => p.Usuario)
                    .Include(d => d.Mascotas)
                    .Where(d => d.Persona != null && d.Persona.ClinicaId == clinicaId.Value)
                    .AsNoTracking();

                // Filtrar por nombre si se proporciona
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    var nombreLower = nombre.ToLower();
                    query = query.Where(d => 
                        (d.Persona!.Nombre + " " + d.Persona.PrimerApellido + " " + (d.Persona.SegundoApellido ?? ""))
                        .ToLower().Contains(nombreLower) ||
                        d.Persona!.Nombre.ToLower().Contains(nombreLower) ||
                        d.Persona!.PrimerApellido.ToLower().Contains(nombreLower) ||
                        (d.Persona.SegundoApellido != null && d.Persona.SegundoApellido.ToLower().Contains(nombreLower))
                    );
                }

                // Contar total de registros
                var totalItems = await query.CountAsync();

                // Calcular paginación
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Obtener datos paginados
                var duenos = await query
                    .OrderBy(d => d.Persona!.Nombre)
                    .ThenBy(d => d.Persona!.PrimerApellido)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new DetalleDuenoDTO
                    {
                        Id = d.Id,
                        Email = d.Persona.Usuario.Email,
                        Telefono = d.Persona.Telefono, // El teléfono no está en el modelo actual
                        Imagen = d.Persona.Imagen
                   })
                    .ToListAsync();

                // Crear resultado paginado
                var resultado = new PaginatedResultDTO<DetalleDuenoDTO>
                {
                    Items = duenos,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };

                respuesta.Status = true;
                respuesta.Response = resultado;
                respuesta.Message = new() { $"Se encontraron {totalItems} dueños." };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Status = false;
                respuesta.Message = new() { "Error al obtener el listado de dueños.", ex.Message };
                return StatusCode(500, respuesta);
            }
        }
    }
}
