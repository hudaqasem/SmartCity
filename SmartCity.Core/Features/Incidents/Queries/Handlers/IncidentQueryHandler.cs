using AutoMapper;
using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Queries.Models;
using SmartCity.AppCore.Features.Incidents.Queries.Results;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Incidents.Queries.Handlers
{
    public class IncidentQueryHandler : ResponseHandler
        , IRequestHandler<GetIncidentQuery, Response<List<GetIncidentListResponse>>>
        , IRequestHandler<GetMyIncidentsQuery, Response<List<GetMyIncidentResponse>>>
        , IRequestHandler<GetIncidentByIdQuery, Response<GetIncidentDetailsResponse>>
        , IRequestHandler<GetAllIncidentsQuery, Response<List<GetIncidentResponse>>>
    {
        #region Fields
        private readonly IIncidentService incidentService;
        private readonly IMapper mapper;

        #endregion

        #region Constructors
        public IncidentQueryHandler(IIncidentService _incidentService,
            IMapper _mapper)
        {
            incidentService = _incidentService;
            mapper = _mapper;
        }



        #endregion

        #region Handle Functions

        public async Task<Response<List<GetIncidentListResponse>>> Handle(GetIncidentQuery request, CancellationToken cancellationToken)
        {
            var incidentList = await incidentService.GetIncidentsAsync();
            var incidentListMapper = mapper.Map<List<GetIncidentListResponse>>(incidentList);
            return Success(incidentListMapper);
        }

        public async Task<Response<List<GetMyIncidentResponse>>> Handle(GetMyIncidentsQuery request, CancellationToken cancellationToken)
        {
            var incidents = await incidentService.GetIncidentByUserIdAsync(request.UserId);
            var mapped = mapper.Map<List<GetMyIncidentResponse>>(incidents);
            return Success(mapped);
        }

        public async Task<Response<GetIncidentDetailsResponse>> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
        {
            var incident = await incidentService.GetByIdAsync(request.Id);
            if (incident == null)
                return NotFound<GetIncidentDetailsResponse>("Incident not found");

            var mapped = mapper.Map<GetIncidentDetailsResponse>(incident);
            return Success(mapped);
        }

        public async Task<Response<List<GetIncidentResponse>>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken)
        {
            var incidents = await incidentService.GetAllFilteredAsync(request.Status, request.Type, request.FromDate, request.ToDate);
            var mapped = mapper.Map<List<GetIncidentResponse>>(incidents);
            return Success(mapped);
        }

        #endregion
    }
}
