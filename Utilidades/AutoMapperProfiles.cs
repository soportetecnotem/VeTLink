using AutoMapper;
using Microsoft.AspNetCore.Identity;
using VeTLink.DTOs.Alergia;
using VeTLink.DTOs.Bano;
using VeTLink.DTOs.Carnet;
using VeTLink.DTOs.Catalogo;
using VeTLink.DTOs.Clinica;
using VeTLink.DTOs.Consulta;
using VeTLink.DTOs.Desparasitacion;
using VeTLink.DTOs.Diagnostico;
using VeTLink.DTOs.Direccion;
using VeTLink.DTOs.Dueno;
using VeTLink.DTOs.Enfermedad;
using VeTLink.DTOs.Exploracion;
using VeTLink.DTOs.HistorialMedico;
using VeTLink.DTOs.HistorialReproductivo;
using VeTLink.DTOs.Mascota;
using VeTLink.DTOs.Modulo;
using VeTLink.DTOs.Persona;
using VeTLink.DTOs.Plan;
using VeTLink.DTOs.Profilaxis;
using VeTLink.DTOs.Sintoma;
using VeTLink.DTOs.Sucursal;
using VeTLink.DTOs.Suscripcion;
using VeTLink.DTOs.Usuario;
using VeTLink.DTOs.Vacuna;
using VeTLink.DTOs.Veterinario;
using VeTLink.Models;

namespace VeTLink.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Persona PersonaDto
            CreateMap<Persona, PersonaDTO>()
               .ReverseMap();

            // Persona DetallePersonaDTO
            CreateMap<Persona, DetallePersonaDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.TipoUsuarioNombre, opt => opt.MapFrom(src => src.TipoUsuario != null ? src.TipoUsuario.Nombre : null))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion));

            // CreatePersonaDTO  Persona
            CreateMap<CreatePersonaDTO, Persona>()
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion));

            // Persona  ListadoPersonaDTO
            CreateMap<Persona, ListadoPersonaDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.TipoUsuarioNombre, opt => opt.MapFrom(src => src.TipoUsuario != null ? src.TipoUsuario.Nombre : null));

            // RegisterDto Persona
            CreateMap<RegisterDto, Persona>();

            // RegisterDto IdentityUser
            CreateMap<RegisterDto, IdentityUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            //Catalogos
            CreateMap<CondicionCorporal, CatalogoDTO>().ReverseMap();
            CreateMap<CondicionCorporal, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<EstadoGeneral, CatalogoDTO>().ReverseMap();
            CreateMap<EstadoGeneral, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<Mucosa, CatalogoDTO>().ReverseMap();
            CreateMap<Mucosa, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<Hidratacion, CatalogoDTO>().ReverseMap();
            CreateMap<Hidratacion, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<Comportamiento, CatalogoDTO>().ReverseMap();
            CreateMap<Comportamiento, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<TipoPrueba, CatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Prueba))
                .ReverseMap();
            CreateMap<TipoPrueba, DetalleCatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Prueba))
                .ReverseMap();

            CreateMap<ViaAdministracion, CatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Via))
                .ReverseMap();
            CreateMap<ViaAdministracion, DetalleCatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Via))
                .ReverseMap();

            CreateMap<TipoTratamiento, CatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Tipo))
                .ReverseMap();
            CreateMap<TipoTratamiento, DetalleCatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Tipo))
                .ReverseMap();

            CreateMap<NivelActividad, CatalogoDTO>().ReverseMap();
            CreateMap<NivelActividad, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<ReaccionSocial, CatalogoDTO>().ReverseMap();
            CreateMap<ReaccionSocial, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<TipoServicio, CatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Servicio))
                .ReverseMap();
            CreateMap<TipoServicio, DetalleCatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Servicio))
                .ReverseMap();

            CreateMap<UnidadTiempo, CatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Unidad))
                .ReverseMap();
            CreateMap<UnidadTiempo, DetalleCatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Unidad))
                .ReverseMap();

            CreateMap<Plan, CatalogoDTO>().ReverseMap();
            CreateMap<Plan, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<EstadoSuscripcion, CatalogoDTO>().ReverseMap();
            CreateMap<EstadoSuscripcion, DetalleCatalogoDTO>().ReverseMap();

            // NUEVOS MAPEOS - Enfermedad y Alergia
            CreateMap<Enfermedad, CatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Nombre))
                .ReverseMap();
            CreateMap<Enfermedad, DetalleCatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Nombre))
                .ReverseMap();

            CreateMap<Alergia, CatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Sustancia))
                .ReverseMap();
            CreateMap<Alergia, DetalleCatalogoDTO>()
                .ForMember(dto => dto.Descripcion, config => config.MapFrom(ent => ent.Sustancia))
                .ReverseMap();

            // Direccion
            CreateMap<DireccionDTO, Direccion>().ReverseMap();
            CreateMap<DetalleDireccionDTO, Direccion>().ReverseMap();

            // Clinica
            CreateMap<RegistroClinicaDTO, Clinica>()
               .ForMember(dto => dto.SuscripcionId, config => config.MapFrom(ent => ent.SuscripcionId));

            CreateMap<Clinica, ClinicaDTO>().ReverseMap();

            // Clinica DetalleClinicaDTO
            CreateMap<Clinica, DetalleClinicaDTO>()
                .ForMember(dto => dto.Suscripcion, config => config.MapFrom(ent => ent.Suscripcion))
                .ForMember(dto => dto.Sucursales, config => config.MapFrom(ent => ent.Sucursales))
                .ReverseMap();

            //Clinica/Sucursales
            CreateMap<Sucursal, SucursalDTO>().ReverseMap();
            CreateMap<CrearSucursalDTO, Sucursal>().ReverseMap();
            CreateMap<Sucursal, UpdateSucursalDTO>()
                .ForMember(dto => dto.Direccion, config => config.MapFrom(ent => ent.Direccion))
                .ReverseMap();
            CreateMap<Sucursal, DetalleSucursalDTO>()
                .ForMember(dto => dto.Direccion, config => config.MapFrom(ent => ent.Direccion))
                .ForMember(dest => dest.NombreClinica, opt => opt.MapFrom(src => src.Clinica!.NombreClinica))
                .ReverseMap();

            // Primer registro de la clinica con su admin clinica
            CreateMap<RegistroClinicaDTO, Persona>()
                .ForMember(dto => dto.Nombre, config => config.MapFrom(ent => ent.Nombre))
                .ForMember(dto => dto.PrimerApellido, config => config.MapFrom(ent => ent.PrimerApellido))
                .ForMember(dto => dto.SegundoApellido, config => config.MapFrom(ent => ent.SegundoApellido))
                .ForMember(dto => dto.Genero, config => config.MapFrom(ent => ent.Genero))
                .ForMember(dto => dto.FechaNacimiento, config => config.MapFrom(ent => ent.FechaNacimiento))
                .ForMember(dto => dto.TipoUsuarioId, config => config.MapFrom(ent => ent.TipoUsuarioId));

            // Veterinario
            CreateMap<RegistroClinicaDTO, Veterinario>()
                .ForMember(dest => dest.CedulaProfesional, opt => opt.MapFrom(src => src.CedulaProfesional))
                .ForMember(dest => dest.Horarios, opt => opt.MapFrom(src => src.Horarios))
                .ForMember(dest => dest.Persona, opt => opt.Ignore());

            CreateMap<Veterinario, VeterinarioDTO>().ReverseMap();

            CreateMap<Veterinario, ListadoVeterinarioDTO>()
                .ForMember(dest => dest.NombreCompleto,
                    opt => opt.MapFrom(src =>
                        $"{src.Persona!.Nombre} {src.Persona.PrimerApellido} {src.Persona.SegundoApellido ?? ""}".Trim()))
                .ForMember(dest => dest.Sucursales,
                    opt => opt.MapFrom(src => src.SucursalesAsignadas
                        .Select(s => s.NombreSucursal ?? "Sin nombre").ToList()))
                .ForMember(dest => dest.Clinicas,
                    opt => opt.MapFrom(src => src.SucursalesAsignadas
                        .Where(s => s.Clinica != null)
                        .Select(s => s.Clinica!.NombreClinica)
                        .Distinct()
                        .ToList()));

            //Suscripcion
            CreateMap<CrearSuscripcionDTO, Suscripcion>()
                .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.EstadoSuscripcionId, opt => opt.MapFrom(_ => 1)); // 1 = Activa por defecto

            CreateMap<Suscripcion, DetalleSuscripcionDTO>()
                .ForMember(dest => dest.EstadoSuscripcionNombre, opt => opt.MapFrom(src => src.EstadoSuscripcion != null ? src.EstadoSuscripcion.Descripcion : null))
                .ForMember(dest => dest.PlanNombre, opt => opt.MapFrom(src => src.Plan != null ? src.Plan.NombrePlan : null))
                .ForMember(dest => dest.NombreClinica, opt => opt.MapFrom(src => src.Clinica != null ? src.Clinica.NombreClinica : null));

            CreateMap<IdentityUser, UsuarioDTO>()
               .ReverseMap();

            CreateMap<CreateVeterinarioDTO, Veterinario>().ReverseMap();
            CreateMap<UpdateVeterinarioDTO, Veterinario>().ReverseMap();
            CreateMap<Veterinario, DetalleVeterinarioDTO>()
                .ForMember(dest => dest.Persona, opt => opt.MapFrom(src => src.Persona)).ReverseMap();

            //Planes modulos
            CreateMap<Plan, PlanDetalleDTO>().ReverseMap();
            CreateMap<Modulo, ModuloDTO>().ReverseMap();
            CreateMap<PlanCrearDTO, Plan>().ReverseMap();
            CreateMap<PlanActualizarDTO, Plan>().ReverseMap();

            // Historial Reproductivo
            CreateMap<HistorialReproductivo, HistorialReproductivoDTO>().ReverseMap();

            // Historial Médico
            CreateMap<HistorialMedico, HistorialMedicoDTO>()
                .ForMember(dest => dest.HistorialReproductivo, opt => opt.MapFrom(src => src.HistorialReproductivo))
                .ForMember(dest => dest.Enfermedades, opt => opt.MapFrom(src => src.Enfermedades.Select(e => e.Enfermedad).ToList()))
                .ForMember(dest => dest.Alergias, opt => opt.MapFrom(src => src.Alergias.Select(a => a.Id).ToList()))
                .ReverseMap();


            // Exploración Física
            CreateMap<ExploracionFisica, ExploracionDTO>().ReverseMap();

            // Diagnóstico
            CreateMap<Diagnostico, DiagnosticoDTO>().ReverseMap();

            // Síntoma Actual
            CreateMap<SintomaActual, SintomaDTO>()
                .ForMember(dest => dest.ComportamientoNombre, opt => opt.MapFrom(src => src.Comportamiento != null ? src.Comportamiento.Descripcion : null))
                .ForMember(dest => dest.UnidadTiempoNombre, opt => opt.MapFrom(src => src.UnidadTiempo != null ? src.UnidadTiempo.Unidad : null))
                .ReverseMap();

            // Consulta médica 
            CreateMap<ConsultaMedicaDTO, ConsultaMedica>(); // Solo para input

            CreateMap<ConsultaMedica, DetallesConsultaMedicaDTO>()
                // Información del veterinario
                .ForMember(dest => dest.NombreVeterinario, opt => opt.MapFrom(src =>
                    src.Veterinario != null && src.Veterinario.Persona != null
                        ? $"{src.Veterinario.Persona.Nombre} {src.Veterinario.Persona.PrimerApellido}"
                        : null))
                .ForMember(dest => dest.CedulaVeterinario, opt => opt.MapFrom(src =>
                    src.Veterinario != null ? src.Veterinario.CedulaProfesional : null))

                // Información del tipo de servicio
                .ForMember(dest => dest.TipoServicioNombre, opt => opt.MapFrom(src =>
                    src.TipoServicio != null ? src.TipoServicio.Servicio : null))

                // Datos completos de la mascota (incluye dueño e historial)
                .ForMember(dest => dest.Mascota, opt => opt.MapFrom(src => src.Mascota))

                // Datos clínicos
                .ForMember(dest => dest.Exploracion, opt => opt.MapFrom(src => src.Exploracion))
                .ForMember(dest => dest.Diagnostico, opt => opt.MapFrom(src => src.Diagnostico))
                .ForMember(dest => dest.Sintomas, opt => opt.MapFrom(src => src.Sintomas));

            // Mapeo de Dueno a DuenoDetalleDTO
            CreateMap<Dueno, DetalleDuenoDTO>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.Nombre : null))
                .ForMember(dest => dest.PrimerApellido, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.PrimerApellido : null))
                .ForMember(dest => dest.SegundoApellido, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.SegundoApellido : null))
                .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.Genero : null))
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.FechaNacimiento : null))
                .ForMember(dest => dest.NumeroIdentificacion, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.NumeroIdentificacion : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Persona != null && src.Persona.Usuario != null ? src.Persona.Usuario.Email : null))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.Telefono : null))
                .ForMember(dest => dest.Imagen, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.Imagen : null))
                .ForMember(dest => dest.ClinicaId, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.ClinicaId : null))
                .ForMember(dest => dest.NombreClinica, opt => opt.MapFrom(src => src.Persona != null && src.Persona.Clinica != null ? src.Persona.Clinica.NombreClinica : null))
                .ForMember(dest => dest.Mascotas, opt => opt.MapFrom(src => src.Mascotas));

            // Mapeos de Mascota
            CreateMap<Mascota, MascotaDTO>().ReverseMap();

            CreateMap<Mascota, DetalleMascotaDTO>()
                .ForMember(dest => dest.NombreDueno, opt => opt.MapFrom(src =>
                    src.Dueno != null && src.Dueno.Persona != null
                        ? $"{src.Dueno.Persona.Nombre} {src.Dueno.Persona.PrimerApellido} {src.Dueno.Persona.SegundoApellido ?? ""}".Trim()
                        : null))
                .ForMember(dest => dest.Dueno, opt => opt.MapFrom(src => src.Dueno))
                .ForMember(dest => dest.HistorialMedico, opt => opt.MapFrom(src => src.HistorialMedico))
                .ForMember(dest => dest.Carnet, opt => opt.MapFrom(src => src.Carnet));

            // Mapeos de Carnet Preventivo
            CreateMap<CarnetPreventivo, CarnetDTO>()
                .ForMember(dest => dest.RegistroVacunas, opt => opt.MapFrom(src => src.RegistroVacunas))
                .ForMember(dest => dest.RegistroDesparasitaciones, opt => opt.MapFrom(src => src.RegistroDesparasitaciones))
                .ForMember(dest => dest.RegistroBanos, opt => opt.MapFrom(src => src.RegistroBanos))
                .ForMember(dest => dest.RegistroProfilaxis, opt => opt.MapFrom(src => src.RegistroProfilaxis));

            CreateMap<Vacuna, VacunaDTO>().ReverseMap();
            CreateMap<Desparasitacion, DesparasitacionDTO>().ReverseMap();
            CreateMap<Bano, BanoDTO>().ReverseMap();
            CreateMap<Profilaxis, ProfilaxisDTO>().ReverseMap();

            // Mascota Consulta con historial
            CreateMap<Mascota, MascotaConsultaDTO>()
                .ForMember(dest => dest.HistorialMedico, opt => opt.MapFrom(src => src.HistorialMedico))
                .ReverseMap();

            //Alergia
            CreateMap<Alergia, AlergiaDTO>().ReverseMap();
        }
    }
}
