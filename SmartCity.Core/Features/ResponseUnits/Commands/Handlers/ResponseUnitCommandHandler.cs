using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.ResponseUnits.Commands.Models;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Handlers
{
    public class ResponseUnitCommandHandler : ResponseHandler,
        IRequestHandler<AcceptAssignmentCommand, Response<string>>,
        IRequestHandler<RejectAssignmentCommand, Response<string>>,
        IRequestHandler<ArriveAtSceneCommand, Response<string>>,
        IRequestHandler<CompleteAssignmentCommand, Response<string>>
    {
        private readonly IUnitAssignmentService _service;
        private readonly IResponseUnitService _responseUnitService;
        private readonly IResponseUnitRepository _unitRepo;
        private readonly IMapper _mapper;

        public ResponseUnitCommandHandler(IUnitAssignmentService service,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IResponseUnitService responseUnitService,
            IResponseUnitRepository unitRepo,
            IMapper mapper)
        {
            _service = service;
            _responseUnitService = responseUnitService;
            _unitRepo = unitRepo;
            _mapper = mapper;
        }

        // Accept Assignment
        public async Task<Response<string>> Handle(AcceptAssignmentCommand request, CancellationToken cancellationToken)
        {

            var unit = await _responseUnitService.GetResponseUnitByUserIdAsync(request.UserId);


            if (unit == null)
                return NotFound<string>("Response unit not found for this user");

            var result = await _service.AcceptAsync(request.IncidentId, unit.Id);

            if (result == "Assignment not found")
                return NotFound<string>(result);

            if (!result.Contains("success", StringComparison.OrdinalIgnoreCase))
                return BadRequest<string>(result);

            return Success(result);
        }

        //  Reject Assignment
        public async Task<Response<string>> Handle(RejectAssignmentCommand request, CancellationToken cancellationToken)
        {
            var unit = await _responseUnitService.GetResponseUnitByUserIdAsync(request.UserId);

            if (unit == null)
                return NotFound<string>("Response unit not found for this user");

            var result = await _service.RejectAsync(request.IncidentId, unit.Id, request.Reason);

            if (result == "Assignment not found")
                return NotFound<string>(result);

            if (!result.Contains("rejected", StringComparison.OrdinalIgnoreCase))
                return BadRequest<string>(result);

            return Success(result);
        }

        // ✅ FIXED: Arrive At Scene
        public async Task<Response<string>> Handle(ArriveAtSceneCommand request, CancellationToken cancellationToken)
        {
            var unit = await _responseUnitService.GetResponseUnitByUserIdAsync(request.UserId);

            if (unit == null)
                return NotFound<string>("Response unit not found for this user");

            var result = await _service.ArriveAsync(request.IncidentId, unit.Id);

            if (result == "Assignment not found")
                return NotFound<string>(result);

            if (!result.Contains("Arrived", StringComparison.OrdinalIgnoreCase))
                return BadRequest<string>(result);

            return Success(result);
        }

        //  Complete Assignment
        public async Task<Response<string>> Handle(CompleteAssignmentCommand request, CancellationToken cancellationToken)
        {
            var unit = await _responseUnitService.GetResponseUnitByUserIdAsync(request.UserId);

            if (unit == null)
                return NotFound<string>("Response unit not found for this user");

            var result = await _service.CompleteAsync(request.IncidentId, unit.Id, request.Notes);

            if (result == "Assignment not found")
                return NotFound<string>(result);

            if (!result.Contains("completed", StringComparison.OrdinalIgnoreCase))
                return BadRequest<string>(result);

            return Success(result);
        }
    }
}