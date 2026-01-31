using AutoMapper;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;
using SmartCity.AppCore.Features.AppUsers.Queries.Results;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Mapping.Auth
{
    public class AppUsersProfile : Profile
    {
        public AppUsersProfile()
        {
            //Add user by admin
            CreateMap<CreateUserCommand, ApplicationUser>();


            //Get All 
            CreateMap<ApplicationUser, GetUserListResponse>();

            // Get by id
            CreateMap<ApplicationUser, GetUserDetailsResponse>();

            // Edit 
            CreateMap<EditUserCommand, ApplicationUser>();


        }
    }
}
