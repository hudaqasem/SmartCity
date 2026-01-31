using FluentValidation;
using SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Validations
{
    public class EditResponseUnitValidator : AbstractValidator<EditResponseUnitCommand>
    {
        public EditResponseUnitValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name too long");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required")
                .Must(x => new[] { "Fire", "Medical", "Police" }.Contains(x))
                .WithMessage("Invalid unit type");

            RuleFor(x => x.Contact)
                .NotEmpty().WithMessage("Contact is required")
                .Matches(@"^[0-9]{10,15}$").WithMessage("Invalid phone number");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
                .WithMessage("Invalid latitude");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
                .WithMessage("Invalid longitude");

        }
    }
}
