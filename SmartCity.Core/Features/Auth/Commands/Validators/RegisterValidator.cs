using FluentValidation;
using SmartCity.AppCore.Features.Auth.Commands.Models;

namespace SmartCity.AppCore.Features.Auth.Commands.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        }
    }
}
