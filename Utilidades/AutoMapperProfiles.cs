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
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));

            CreateMap<Persona, DetallePersonaDTO>()
            .ForMember(dest => dest.TipoUsuarioNombre,
                       opt => opt.MapFrom(src => src.TipoUsuario != null ? src.TipoUsuario.Nombre : null));

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

            CreateMap<TipoPrueba, CatalogoDTO>().ReverseMap();
            CreateMap<TipoPrueba, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<ViaAdministracion, CatalogoDTO>().ReverseMap();
            CreateMap<ViaAdministracion, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<TipoTratamiento, CatalogoDTO>().ReverseMap();
            CreateMap<TipoTratamiento, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<NivelActividad, CatalogoDTO>().ReverseMap();
            CreateMap<NivelActividad, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<ReaccionSocial, CatalogoDTO>().ReverseMap();
            CreateMap<ReaccionSocial, DetalleCatalogoDTO>().ReverseMap();

            CreateMap<TipoServicio, CatalogoDTO>().ReverseMap();
            CreateMap<TipoServicio, DetalleCatalogoDTO>().ReverseMap();
        }
    }
}
