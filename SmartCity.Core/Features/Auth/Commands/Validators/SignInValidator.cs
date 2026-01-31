using FluentValidation;
using SmartCity.AppCore.Features.Auth.Commands.Models;

namespace SmartCity.AppCore.Features.Auth.Commands.Validators
{
    public class SignInValidator : AbstractValidator<SignInCommand>
    {
        public SignInValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User name is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
