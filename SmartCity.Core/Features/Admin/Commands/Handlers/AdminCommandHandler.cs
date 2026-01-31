using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Admin.Commands.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Admin.Commands.Handlers
{
    public class AdminCommandHandler : ResponseHandler,
        IRequestHandler<ManualAssignCommand, Response<string>>
    {
        private readonly IUnitAssignmentService _assignmentService;

        public AdminCommandHandler(IUnitAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        public async Task<Response<string>> Handle(
            ManualAssignCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _assignmentService.ManualAssignAsync(
                request.IncidentId,
                request.UnitId
            );

            if (!result)
                return BadRequest<string>("Manual assignment failed. Unit may be unavailable.");

            return Success("Unit assigned successfully");
        }
    }
}
