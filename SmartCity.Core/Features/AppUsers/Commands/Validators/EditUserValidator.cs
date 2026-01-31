using FluentValidation;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Validators
{
    public class EditUserValidator : AbstractValidator<EditUserCommand>
    {
        public EditUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Id).NotEmpty().NotNull();
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        }
    }
}


