using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Handlers
{
    public class ResponseUnitAdminHandler : ResponseHandler,
        IRequestHandler<CreateResponseUnitCommand, Response<string>>,
        IRequestHandler<EditResponseUnitCommand, Response<string>>,
        IRequestHandler<AssignUserToUnitCommand, Response<string>>,
        IRequestHandler<ChangeUnitStatusCommand, Response<string>>,
        IRequestHandler<EnableDisableUnitCommand, Response<string>>
    {
        private readonly IAdminResponseUnitService _adminResponseUnitService;
        private readonly IResponseUnitService _responseUnitService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResponseUnitAdminHandler(IAdminResponseUnitService adminResponseUnitService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IResponseUnitService responseUnitService)
        {
            _adminResponseUnitService = adminResponseUnitService;
            _mapper = mapper;
            _userManager = userManager;
            _responseUnitService = responseUnitService;
        }





        public async Task<Response<string>> Handle(CreateResponseUnitCommand request, CancellationToken cancellationToken)
        {

            // create the ApplicationUser
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.Contact,
                FirstName = request.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createUserResult = await _userManager.CreateAsync(user, request.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = string.Join("; ", createUserResult.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            // assign role
            var addRoleResult = await _userManager.AddToRoleAsync(user, "ResponseUnit");
            if (!addRoleResult.Succeeded)
            {
                // rollback user creation? here we can delete created user to keep consistency:
                await _userManager.DeleteAsync(user);
                return BadRequest<string>("User created but failed to assign ResponseUnit role.");
            }

            // create ResponseUnit entity and link UserId
            var unit = new ResponseUnit
            {
                Name = request.Name,
                Type = request.Type,
                Contact = request.Contact,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsActive = true,
                Status = UnitStatus.Available,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _adminResponseUnitService.AddAsync(unit);

            return Created("Response unit created and user account provisioned successfully.");
        }

        public async Task<Response<string>> Handle(EditResponseUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _responseUnitService.GetResponseUnitByIdAsync(request.Id);
            if (unit == null)
                return NotFound<string>("Response Unit not found");

            var unitMap = _mapper.Map<ResponseUnit>(request);
            var res = await _adminResponseUnitService.EditAsync(unitMap);

            if (res == "Success")
                return Success("Response Unit Updated Successfully");

            return BadRequest<string>();
        }

        public async Task<Response<string>> Handle(AssignUserToUnitCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminResponseUnitService.AssignUserAsync(request.UnitId, request.UserId);
            return Success(result);
        }

        public async Task<Response<string>> Handle(ChangeUnitStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminResponseUnitService.ChangeStatusAsync(request.UnitId, request.Status);
            return Success(result);
        }

        public async Task<Response<string>> Handle(EnableDisableUnitCommand request, CancellationToken cancellationToken)
        {

            var result = await _adminResponseUnitService.ToggleActiveAsync(request.UnitId, request.IsActive);
            return Success(result);
        }


    }
}
