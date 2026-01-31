using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Admin.Queries.Models;
using SmartCity.AppCore.Features.Admin.Queries.Results;
using SmartCity.Domain.Enums;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Admin.Queries.Handler
{
    public class AdminQueryHandler : ResponseHandler,
        IRequestHandler<GetWaitingIncidentsQuery, Response<List<WaitingIncidentResponse>>>,
        IRequestHandler<GetAvailableUnitsQuery, Response<List<UnitAvailabilityResponse>>>,
        IRequestHandler<GetNearestUnitsQuery, Response<List<NearestUnitResponse>>>
    {
        private readonly IIncidentRepository _incidentRepo;
        private readonly IResponseUnitRepository _unitRepo;
        private readonly IUnitDistanceService _distanceService;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public AdminQueryHandler(
            IIncidentRepository incidentRepo,
            IResponseUnitRepository unitRepo,
            IUnitDistanceService distanceService,
            ApplicationDbContext db,
            IMapper mapper)
        {
            _incidentRepo = incidentRepo;
            _unitRepo = unitRepo;
            _distanceService = distanceService;
            _db = db;
            _mapper = mapper;
        }

        // Get all incidents waiting for units
        public async Task<Response<List<WaitingIncidentResponse>>> Handle(
            GetWaitingIncidentsQuery request,
            CancellationToken cancellationToken)
        {
            var incidents = await _db.Incidents
                .Include(i => i.ReportedByUser)
                .Where(i => i.Status == IncidentStatus.WaitingForUnit)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();

            var result = incidents.Select(i => new WaitingIncidentResponse
            {
                Id = i.Id,
                Type = i.Type,
                Description = i.Description,
                Location = $"{i.Latitude}, {i.Longitude}",
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                CreatedAt = i.CreatedAt,
                ReportedBy = i.ReportedByUser != null
                    ? $"{i.ReportedByUser.FirstName} {i.ReportedByUser.LastName}"
                    : "Unknown"
            }).ToList();

            return Success(result);
        }

        // Get available units (optionally filtered by type)
        public async Task<Response<List<UnitAvailabilityResponse>>> Handle(
            GetAvailableUnitsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _db.ResponseUnits
                .Where(u => u.IsActive && u.Status == UnitStatus.Available);

            if (!string.IsNullOrEmpty(request.Type))
            {
                query = query.Where(u => u.Type == request.Type);
            }

            var units = await query.ToListAsync();
            var result = _mapper.Map<List<UnitAvailabilityResponse>>(units);

            return Success(result);
        }

        // Get nearest units for a specific incident
        public async Task<Response<List<NearestUnitResponse>>> Handle(
            GetNearestUnitsQuery request,
            CancellationToken cancellationToken)
        {
            var incident = await _incidentRepo.GetByIdAsync(request.IncidentId);
            if (incident == null)
                return NotFound<List<NearestUnitResponse>>("Incident not found");

            if (!incident.Latitude.HasValue || !incident.Longitude.HasValue)
                return BadRequest<List<NearestUnitResponse>>("Incident has no location");

            // Get all units of same type
            var units = await _db.ResponseUnits
                .Where(u => u.Type == incident.Type &&
                           u.IsActive &&
                           u.Latitude.HasValue &&
                           u.Longitude.HasValue)
                .ToListAsync();

            // Calculate distances
            var nearestUnits = units
                .Select(u => new NearestUnitResponse
                {
                    UnitId = u.Id,
                    UnitName = u.Name,
                    Type = u.Type,
                    Status = u.Status,
                    Contact = u.Contact,
                    Distance = _distanceService.CalculateDistance(
                        incident.Latitude.Value,
                        incident.Longitude.Value,
                        u.Latitude.Value,
                        u.Longitude.Value
                    )
                })
                .OrderBy(x => x.Distance)
                .Take(request.TopCount)
                .ToList();

            return Success(nearestUnits);
        }
    }
}
