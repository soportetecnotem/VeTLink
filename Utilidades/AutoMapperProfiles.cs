using AutoMapper;
using Microsoft.AspNetCore.Identity;
using VeTLink.DTOs;
using VeTLink.Models;

namespace VeTLink.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Persona PersonaDto
            CreateMap<Persona, PersonaDTO>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email)).ReverseMap();

            // Persona DetallePersonaDTO
            CreateMap<Persona, DetallePersonaDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.TipoUsuarioNombre, opt => opt.MapFrom(src => src.TipoUsuario != null ? src.TipoUsuario.Nombre : null))
                .ForMember(dest => dest.DireccionDTO, opt => opt.MapFrom(src => src.Direccion));

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

            // Direccion
            CreateMap<DireccionDTO, Direccion>().ReverseMap();
            CreateMap<DetalleDireccionDTO, Direccion>().ReverseMap();

            // Clinica
            CreateMap<RegistroClinicaDTO, Clinica>()
                .ForMember(dto => dto.Direccion, config => config.MapFrom(ent => ent.Direccion))
                .ForMember(dto => dto.Email, config => config.MapFrom(ent => ent.EmailClinica))
                .ForMember(dto => dto.SuscripcionId, config => config.MapFrom(ent => ent.SuscripcionId));
            
            CreateMap<Clinica, ClinicaDTO>().ReverseMap();

            CreateMap<Clinica, DetalleClinicaDTO>()
                .ForMember(dto => dto.SuscripcionId, config => config.MapFrom(ent => ent.SuscripcionId))
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
                .ForMember(dest => dest.Persona, opt => opt.Ignore())
                .ForMember(dest => dest.ClinicasAsignadas, opt => opt.Ignore());

            CreateMap<Veterinario, VeterinarioDTO>().ReverseMap();

            //Suscripcion
            CreateMap<CrearSuscripcionDTO, Suscripcion>()
                .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.EstadoSuscripcionId, opt => opt.MapFrom(_ => 1)); // 1 = Activa por defecto

            CreateMap<Suscripcion, DetalleSuscripcionDTO>()
                .ForMember(dest => dest.EstadoSuscripcionNombre, opt => opt.MapFrom(src => src.EstadoSuscripcion != null ? src.EstadoSuscripcion.Descripcion : null))
                .ForMember(dest => dest.PlanNombre, opt => opt.MapFrom(src => src.Plan != null ? src.Plan.NombrePlan : null))
                .ForMember(dest => dest.NombreClinica, opt => opt.MapFrom(src => src.Clinica != null ? src.Clinica.NombreClinica : null));
        }
    }
}
