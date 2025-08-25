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
        }
    }
}
