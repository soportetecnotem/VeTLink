using AutoMapper;
using VeTLink.DTOs.Consulta;
using VeTLink.Models;

namespace VeTLink.Profiles
{
    public class ConsultaMedicaProfile : Profile
    {
        public ConsultaMedicaProfile()
        {
            CreateMap<ConsultaMedicaDTO, ConsultaMedica>();

            CreateMap<ConsultaMedica, DetallesConsultaMedicaDTO>()
                .ForMember(dest => dest.NombreMascota, opt => opt.MapFrom(src => src.Mascota != null ? src.Mascota.Nombre : null))
                .ForMember(dest => dest.NombreVeterinario, opt => opt.MapFrom(src => 
                    src.Veterinario != null && src.Veterinario.Persona != null 
                        ? $"{src.Veterinario.Persona.Nombre} {src.Veterinario.Persona.PrimerApellido}" 
                        : null))
                .ForMember(dest => dest.CedulaVeterinario, opt => opt.MapFrom(src => src.Veterinario != null ? src.Veterinario.CedulaProfesional : null))
                .ForMember(dest => dest.TipoServicio, opt => opt.MapFrom(src => src.TipoServicio != null ? src.TipoServicio.Servicio : null));
        }
    }
}