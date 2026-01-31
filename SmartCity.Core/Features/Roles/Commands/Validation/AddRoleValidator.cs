using FluentValidation;
using SmartCity.AppCore.Features.Roles.Commands.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Roles.Commands.Validation
{
    public class AddRoleValidators : AbstractValidator<AddRoleCommand>
    {
        #region Fields
        private readonly IRoleManagementService _roleManagementService;
        #endregion
        #region Constructors

        #endregion
        public AddRoleValidators(IRoleManagementService roleManagementService)
        {
            _roleManagementService = roleManagementService;
            ApplyValidationsRules();
            ApplyCustomValidationsRules();
        }

        #region Actions
        public void ApplyValidationsRules()
        {
            RuleFor(x => x.RoleName)
                 .NotEmpty().WithMessage("Role name is required");
        }

        public void ApplyCustomValidationsRules()
        {
            RuleFor(x => x.RoleName)
                .MustAsync(async (Key, CancellationToken) => !await _roleManagementService.IsRoleExistByName(Key))
                .WithMessage("Role is already exist");
        }

        #endregion
    }
}
