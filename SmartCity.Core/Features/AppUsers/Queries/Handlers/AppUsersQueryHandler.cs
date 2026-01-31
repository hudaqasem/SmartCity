using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.AppUsers.Queries.Models;
using SmartCity.AppCore.Features.AppUsers.Queries.Results;
using SmartCity.Domain.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.AppUsers.Queries.Handlers
{
    public class AppUsersQueryHandler : ResponseHandler
        , IRequestHandler<GetUserListQuery, Response<List<GetUserListResponse>>>
        , IRequestHandler<GetUserByIdQuery, Response<GetUserDetailsResponse>>
        , IRequestHandler<GetUserRolesQuery, Response<GetUserRolesResponse>>


    {

        #region Fields
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly IAdminService adminService;

        #endregion

        #region Constructors
        public AppUsersQueryHandler(UserManager<ApplicationUser> _userManager
            , IAdminService _admin
            , IMapper _mapper)
        {
            userManager = _userManager;
            mapper = _mapper;
            adminService = _admin;
        }



        #endregion

        #region Handle Functions

        public async Task<Response<List<GetUserListResponse>>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users.ToListAsync();
            var userMapper = mapper.Map<List<GetUserListResponse>>(user);
            return Success(userMapper);
        }

        public async Task<Response<GetUserDetailsResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user == null)
                return NotFound<GetUserDetailsResponse>("User not found");

            var mapped = mapper.Map<GetUserDetailsResponse>(user);
            return Success(mapped);
        }

        public async Task<Response<GetUserRolesResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            // Find user
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound<GetUserRolesResponse>("User not found");

            // Get user roles
            var roles = await userManager.GetRolesAsync(user);

            // Build response
            var response = new GetUserRolesResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            return Success(response);
        }

        #endregion
    }

}
