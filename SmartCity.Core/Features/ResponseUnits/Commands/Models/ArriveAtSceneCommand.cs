using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.ResponseUnits.Commands.Models
{
    public class ArriveAtSceneCommand : IRequest<Response<string>>
    {
        public int IncidentId { get; set; }
        public string? UserId { get; set; }
    }
}
