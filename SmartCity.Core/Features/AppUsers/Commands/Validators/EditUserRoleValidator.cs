using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Validators
{
    public class EditUserRoleValidator : AbstractValidator<EditUserRoleCommand>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditUserRoleValidator(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;

            ApplyValidationsRules();
        }

        public void ApplyValidationsRules()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .NotNull().WithMessage("User ID cannot be null");

            RuleFor(x => x.NewRole)
                .NotEmpty().WithMessage("Role name is required")
                .NotNull().WithMessage("Role name cannot be null")
                .MustAsync(async (role, cancellationToken) => await RoleExists(role))
                .WithMessage("The specified role does not exist");
        }

        private async Task<bool> RoleExists(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}
