using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.Domain.Enums;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin
{
    public class ChangeUnitStatusCommand : IRequest<Response<string>>
    {
        public int UnitId { get; set; }
        public UnitStatus Status { get; set; }
    }
}
