using MediatR;
using SmartCity.AppCore.Bases;

namespace SmartCity.AppCore.Features.Admin.Commands.Models
{
    public class ManualAssignCommand : IRequest<Response<string>>
    {
        public int IncidentId { get; set; }
        public int UnitId { get; set; }
    }
}
