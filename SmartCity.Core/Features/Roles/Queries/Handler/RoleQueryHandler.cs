using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Roles.Queries.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Roles.Queries.Handlers
{
    public class RoleQueryHandler : ResponseHandler,
        IRequestHandler<GetAllRolesQuery, Response<List<IdentityRole>>>,
        IRequestHandler<GetRoleByIdQuery, Response<IdentityRole>>
    {
        #region Fields

        private readonly IRoleManagementService _roleService;

        #endregion

        #region Constructor
        public RoleQueryHandler(IRoleManagementService roleService)
        {
            _roleService = roleService;
        }

        #endregion

        #region Handle Functions
        public async Task<Response<List<IdentityRole>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetRolesAsync();
            return Success(roles);
        }

        public async Task<Response<IdentityRole>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetRoleByIdAsync(request.Id);

            if (role == null)
                return NotFound<IdentityRole>("Role not found");

            return Success(role);
        }
        #endregion
    }
}
