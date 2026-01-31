using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin
{
    public class AssignUserToUnitCommand : IRequest<Response<string>>
    {
        public int UnitId { get; set; }
        public string UserId { get; set; }
    }
}
