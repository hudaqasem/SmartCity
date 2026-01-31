using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;
using SmartCity.Domain.Models;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Handlers
{
    public class AppUsersCommandHandler : ResponseHandler,
        IRequestHandler<CreateUserCommand, Response<string>>,
        IRequestHandler<EditUserCommand, Response<string>>,
        IRequestHandler<EditMyProfileCommand, Response<string>>,
        IRequestHandler<EditUserRoleCommand, Response<string>>,
        IRequestHandler<DeleteUserCommand, Response<string>>,
        IRequestHandler<ChangeUserPasswordCommand, Response<string>>

    {

        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor
        public AppUsersCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        #endregion

        #region Handle Functions

        public async Task<Response<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if Role Exists
            var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
            if (!roleExists)
                return BadRequest<string>("Invalid Role");

            // Map user
            var newUser = _mapper.Map<ApplicationUser>(request);
            newUser.UserName = request.Email;


            // Create User
            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            // Assign Role
            var assignRoleResult = await _userManager.AddToRoleAsync(newUser, request.RoleName);
            if (!assignRoleResult.Succeeded)
            {
                return BadRequest<string>("User created but role assignment failed");
            }

            return Created("User Created Successfully");
        }

        public async Task<Response<string>> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var oldUser = await _userManager.FindByIdAsync(request.Id);
            if (oldUser == null)
                return NotFound<string>("User not found");

            var newUser = _mapper.Map(request, oldUser);
            newUser.Email = request.Email;
            newUser.UserName = request.Email;
            var result = await _userManager.UpdateAsync(newUser);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }
            return Success("Updated Successfully");

        }

        public async Task<Response<string>> Handle(EditMyProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
                return NotFound<string>("User not found");

            // Update only the allowed fields
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.UserName = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            return Success("Profile Updated Successfully");
        }

        public async Task<Response<string>> Handle(EditUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound<string>("User not found");

            // Validate Role
            var roleExists = await _roleManager.RoleExistsAsync(request.NewRole);
            if (!roleExists)
                return BadRequest<string>("Invalid Role");

            // Get old roles
            var oldRoles = await _userManager.GetRolesAsync(user);

            // Remove old roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, oldRoles);
            if (!removeResult.Succeeded)
                return BadRequest<string>("Failed to remove previous roles");

            // Assign new role
            var addResult = await _userManager.AddToRoleAsync(user, request.NewRole);
            if (!addResult.Succeeded)
                return BadRequest<string>("Failed to assign new role");

            return Success("User role updated successfully");
        }


        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var User = await _userManager.FindByIdAsync(request.Id);
            if (User == null)
                return NotFound<string>("User not found");
            var result = await _userManager.DeleteAsync(User);
            if (!result.Succeeded)
                return BadRequest<string>();
            return Deleted<string>();
        }

        public async Task<Response<string>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) return NotFound<string>();
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest<string>(result.Errors.FirstOrDefault().Description);
            return Success("Password Updated Successfully");
        }




        #endregion


    }
}
