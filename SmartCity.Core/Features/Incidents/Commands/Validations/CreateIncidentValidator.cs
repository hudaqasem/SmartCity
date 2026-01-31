using FluentValidation;
using SmartCity.AppCore.Features.Incidents.Commands.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Incidents.Commands.Validations
{
    public class CreateIncidentValidator : AbstractValidator<CreateIncidentCommand>
    {
        #region Fields
        //private readonly IIncidentService _incidentService;

        #endregion

        #region Constructor
        public CreateIncidentValidator(IIncidentService incidentService)
        {
            //_incidentService = incidentService;
            ApplyValidationRules();
        }
        #endregion

        #region Actions

        public void ApplyValidationRules()
        {
            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("You Must Enter Less Than 250 Letter");
        }

        #endregion
    }
}
