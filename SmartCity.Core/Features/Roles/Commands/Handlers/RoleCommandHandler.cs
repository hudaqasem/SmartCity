using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Roles.Commands.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Roles.Commands.Handlers
{
    public class RoleCommandHandler : ResponseHandler,
       IRequestHandler<AddRoleCommand, Response<string>>,
        IRequestHandler<EditRoleCommand, Response<string>>,
        IRequestHandler<DeleteRoleCommand, Response<string>>

    {
        #region Fields
        private readonly IRoleManagementService _roleManagementService;

        #endregion

        #region Constructors
        public RoleCommandHandler(IRoleManagementService roleManagementService)
        {
            _roleManagementService = roleManagementService;
        }

        #endregion

        #region Handle Functions

        public async Task<Response<string>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _roleManagementService.AddRoleAsync(request.RoleName);
            if (result == "Success") return Success("Role Created Successfully");
            return BadRequest<string>("Failed to create role");
        }

        public async Task<Response<string>> Handle(EditRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _roleManagementService.EditRoleAsync(request.Id, request.NewRoleName);

            if (result == "CannotEditSystemRole")
                return BadRequest<string>("Cannot edit system roles (Admin, Citizen, ResponseUnit)");

            if (result == "NotFound")
                return NotFound<string>("Role Not Found");

            if (result == "Updated")
                return Success("Role Updated");

            return BadRequest<string>("Failed to update role");

        }

        public async Task<Response<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _roleManagementService.DeleteRoleAsync(request.Id);

            if (result == "CannotDeleteSystemRole")
                return BadRequest<string>("Cannot delete system roles (Admin, Citizen, ResponseUnit)");

            if (result.StartsWith("RoleHasUsers:"))
            {
                var count = result.Split(':')[1];
                return BadRequest<string>($"Cannot delete role. {count} users are assigned to this role.");
            }

            if (result == "Deleted")
                return Success("Role Deleted");

            if (result == "NotFound")
                return NotFound<string>("Role Not Found");

            return BadRequest<string>("Failed to delete role");

        }
        #endregion

    }
}
