using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.Domain.Results;

namespace SmartCity.AppCore.Features.Dashboard.Queries.Models
{
    public class GetDashboardSummaryQuery : IRequest<Response<DashboardSummaryResponse>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
