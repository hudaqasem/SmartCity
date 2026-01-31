using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Queries.Results;
using SmartCity.Domain.Enums;

namespace SmartCity.AppCore.Features.Incidents.Queries.Models
{
    public class GetAllIncidentsQuery : IRequest<Response<List<GetIncidentResponse>>>
    {
        public IncidentStatus? Status { get; set; }
        public string? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
