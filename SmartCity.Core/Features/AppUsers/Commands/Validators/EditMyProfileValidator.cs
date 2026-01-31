using FluentValidation;
using SmartCity.AppCore.Features.AppUsers.Commands.Models;

namespace SmartCity.AppCore.Features.AppUsers.Commands.Validators
{
    public class EditMyProfileValidator : AbstractValidator<EditMyProfileCommand>
    {
        public EditMyProfileValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty();

        }
    }
}
