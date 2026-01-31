using FluentValidation;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Validators
{
    public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.CurrentPassword)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.NewPassword)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.ConfirmPassword)
                 .Equal(x => x.NewPassword).WithMessage("Password Not Equal Confirm Password");

        }
    }
}
