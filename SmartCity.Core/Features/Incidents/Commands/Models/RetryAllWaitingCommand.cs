using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Incidents.Commands.Models
{
    public class RetryAllWaitingCommand : IRequest<Response<int>>
    {
    }
}
