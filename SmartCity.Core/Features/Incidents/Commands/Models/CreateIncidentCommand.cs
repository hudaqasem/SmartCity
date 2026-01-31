using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Commands.Results;

namespace SmartCity.AppCore.Features.Incidents.Commands.Models
{
    public class CreateIncidentCommand : IRequest<Response<CreateIncidentResponse>>
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ReportedByUserId { get; set; }
    }
}
