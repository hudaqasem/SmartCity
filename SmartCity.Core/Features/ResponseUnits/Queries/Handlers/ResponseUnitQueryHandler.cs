using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.ResponseUnits.Queries.Models;
using SmartCity.AppCore.Features.ResponseUnits.Queries.Results;
using SmartCity.Domain.Enums;
using SmartCity.Infrastructure.Data;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.ResponseUnits.Queries.Handlers
{
    public class ResponseUnitQueryHandler : ResponseHandler,
        IRequestHandler<GetResponseUnitQuery, Response<List<GetUnitListResponse>>>,
        IRequestHandler<GetUnitAssignmentsQuery, Response<List<UnitAssignmentResponse>>>
    {
        private readonly IResponseUnitService _responseUnitService;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ResponseUnitQueryHandler(
            IResponseUnitService responseUnitService,
            ApplicationDbContext db,
            IMapper mapper)
        {
            _responseUnitService = responseUnitService;
            _db = db;
            _mapper = mapper;
        }

        public async Task<Response<List<GetUnitListResponse>>> Handle(
            GetResponseUnitQuery request,
            CancellationToken cancellationToken)
        {
            var unitList = await _responseUnitService.GetResponseUnitsAsync();
            var unitListMapper = _mapper.Map<List<GetUnitListResponse>>(unitList);
            return Success(unitListMapper);
        }

        //  Get Unit Assignments
        public async Task<Response<List<UnitAssignmentResponse>>> Handle(
            GetUnitAssignmentsQuery request,
            CancellationToken cancellationToken)
        {

            var unit = await _responseUnitService.GetResponseUnitByUserIdAsync(request.UserId);


            if (unit == null)
                return NotFound<List<UnitAssignmentResponse>>("Response unit not found for this user");

            var query = _db.Assignments
                .Include(a => a.Incident)
                    .ThenInclude(i => i.ReportedByUser)
                .Where(a => a.UnitId == unit.Id);

            // Filter active only (Assigned, Accepted, not Completed)
            if (request.ActiveOnly)
            {
                query = query.Where(a =>
                    a.Status == AssignmentStatus.Assigned ||
                    a.Status == AssignmentStatus.Accepted);
            }

            var assignments = await query
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync(cancellationToken);

            var result = assignments.Select(a => new UnitAssignmentResponse
            {
                AssignmentId = a.IncidentId,
                IncidentId = a.IncidentId,
                IncidentType = a.Incident?.Type ?? "Unknown",
                IncidentDescription = a.Incident?.Description ?? "",
                IncidentLatitude = a.Incident?.Latitude,
                IncidentLongitude = a.Incident?.Longitude,
                IncidentLocation = a.Incident != null
                    ? $"{a.Incident.Latitude}, {a.Incident.Longitude}"
                    : "Unknown",
                Status = a.Status,
                IncidentStatus = a.Incident?.Status ?? IncidentStatus.Reported,
                AssignedAt = a.AssignedAt,
                AcceptedAt = a.AcceptedAt,
                CompletedAt = a.CompletedAt,
                ReportedBy = a.Incident?.ReportedByUser != null
                    ? $"{a.Incident.ReportedByUser.FirstName} {a.Incident.ReportedByUser.LastName}"
                    : "Unknown"
            }).ToList();

            return Success(result);
        }
    }
}