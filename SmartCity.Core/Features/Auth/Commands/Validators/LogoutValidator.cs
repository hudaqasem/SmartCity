using FluentValidation;
using SmartCity.AppCore.Features.Auth.Commands.Models;

namespace SmartCity.AppCore.Features.Auth.Commands.Validators
{
    public class LogoutValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutValidator()
        {
            ApplyValidationsRules();
        }

        public void ApplyValidationsRules()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Access token is required")
                .NotNull().WithMessage("Access token cannot be null");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required")
                .NotNull().WithMessage("Refresh token cannot be null");
        }
    }
}