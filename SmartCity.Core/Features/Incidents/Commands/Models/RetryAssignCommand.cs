using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Incidents.Commands.Models
{
    public class RetryAssignCommand : IRequest<Response<string>>
    {
        public int IncidentId { get; set; }
        public RetryAssignCommand(int id)
        {
            IncidentId = id;
        }
    }
}
