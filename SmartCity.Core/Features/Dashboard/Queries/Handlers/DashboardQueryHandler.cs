using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Dashboard.Queries.Models;
using SmartCity.Domain.Results;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Dashboard.Queries.Handlers
{
    public class DashboardQueryHandler : ResponseHandler,
        IRequestHandler<GetDashboardSummaryQuery, Response<DashboardSummaryResponse>>
    {
        private readonly IDashboardService _dashboardService;

        public DashboardQueryHandler(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<Response<DashboardSummaryResponse>> Handle(
            GetDashboardSummaryQuery request,
            CancellationToken cancellationToken)
        {

            var summary = await _dashboardService.GetDashboardSummaryAsync(
                request.StartDate,
                request.EndDate
            );

            return Success(summary);
        }


    }
}
