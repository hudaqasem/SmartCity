using AutoMapper;
using SmartCity.AppCore.Features.Auth.Commands.Models;
using SmartCity.AppCore.Features.Auth.Commands.Results;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Mapping.Auth
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterCommand, ApplicationUser>();
            CreateMap<ApplicationUser, RegisterResponse>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            //*******************************************************



        }
    }
}
