using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Models.Admin
{
    public class EnableDisableUnitCommand : IRequest<Response<string>>
    {
        public int UnitId { get; set; }
        public bool IsActive { get; set; }
    }
}
