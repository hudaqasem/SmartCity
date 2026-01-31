using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;
using SmartCity.Service.Abstracts;

namespace SmartCity.Service.Implementations
{
    public class UnitAssignmentService : IUnitAssignmentService
    {
        private readonly IResponseUnitRepository _unitRepo;
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly IIncidentRepository _incidentRepo;
        private readonly IUnitDistanceService _distanceService;
        private readonly ApplicationDbContext _db;

        public UnitAssignmentService(
            IResponseUnitRepository unitRepo,
            IAssignmentRepository assignmentRepo,
            IIncidentRepository incidentRepo,
            IUnitDistanceService distanceService,
            ApplicationDbContext db)
        {
            _unitRepo = unitRepo;
            _assignmentRepo = assignmentRepo;
            _incidentRepo = incidentRepo;
            _distanceService = distanceService;
            _db = db;
        }

        //  Auto Assign Unit
        public async Task<bool> AutoAssignUnitAsync(Incident incident)
        {
            if (!incident.Latitude.HasValue || !incident.Longitude.HasValue)
            {

                var trackedIncident = await _db.Incidents.FindAsync(incident.Id);
                if (trackedIncident != null)
                {
                    trackedIncident.Status = IncidentStatus.WaitingForUnit;
                    trackedIncident.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
                return false;
            }

            // 1) Get available units of same type
            var units = await _unitRepo.GetAvailableUnitsByTypeAsync(incident.Type);

            // 2) No available units → mark as waiting
            if (!units.Any())
            {
                var trackedIncident = await _db.Incidents.FindAsync(incident.Id);
                if (trackedIncident != null)
                {
                    trackedIncident.Status = IncidentStatus.WaitingForUnit;
                    trackedIncident.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
                return false;
            }

            // 3) Calculate distance for each unit
            var nearest = units
                .Select(u => new
                {
                    Unit = u,
                    Distance = _distanceService.CalculateDistance(
                        incident.Latitude.Value,
                        incident.Longitude.Value,
                        u.Latitude.Value,
                        u.Longitude.Value
                    )
                })
                .OrderBy(x => x.Distance)
                .First().Unit;

            // 4) Begin transaction
            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var assignment = new Assignment
                {
                    IncidentId = incident.Id,
                    UnitId = nearest.Id,
                    Status = AssignmentStatus.Assigned,
                    AssignedAt = DateTime.UtcNow
                };

                await _assignmentRepo.AddAsync(assignment);

                // Update Unit status to EnRoute
                nearest.Status = UnitStatus.EnRoute;
                _db.ResponseUnits.Update(nearest);

                //  Update incident properly
                var trackedIncident = await _db.Incidents.FindAsync(incident.Id);
                if (trackedIncident != null)
                {
                    trackedIncident.Status = IncidentStatus.Assigned;
                    trackedIncident.UpdatedAt = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> RetryAssignAsync(int incidentId)
        {
            var incident = await _db.Incidents.FindAsync(incidentId);
            if (incident == null) return false;

            if (incident.Status != IncidentStatus.WaitingForUnit)
                return false;

            return await AutoAssignUnitAsync(incident);
        }

        public async Task<int> RetryAllWaitingAsync()
        {
            var waitingIncidents = await _incidentRepo.GetWaitingForUnitAsync();

            int successCount = 0;

            foreach (var incident in waitingIncidents)
            {
                var result = await AutoAssignUnitAsync(incident);
                if (result) successCount++;
            }

            return successCount;
        }

        public async Task<bool> ManualAssignAsync(int incidentId, int unitId)
        {
            var incident = await _incidentRepo.GetByIdAsync(incidentId);
            if (incident == null) return false;

            var unit = await _unitRepo.GetByIdAsync(unitId);
            if (unit == null || unit.Status != UnitStatus.Available || !unit.IsActive)
                return false;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var assignment = new Assignment
                {
                    IncidentId = incidentId,
                    UnitId = unitId,
                    Status = AssignmentStatus.Assigned,
                    AssignedAt = DateTime.UtcNow
                };

                await _assignmentRepo.AddAsync(assignment);

                unit.Status = UnitStatus.EnRoute;
                _db.ResponseUnits.Update(unit);

                incident.Status = IncidentStatus.Assigned;
                incident.UpdatedAt = DateTime.UtcNow;
                _db.Incidents.Update(incident);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                return false;
            }
        }

        // Accept Assignment
        public async Task<string> AcceptAsync(int incidentId, int unitId)
        {
            var assignment = await _assignmentRepo.GetByCompositeKeyAsync(incidentId, unitId);
            if (assignment == null) return "Assignment not found";

            if (assignment.Status != AssignmentStatus.Assigned)
                return "Assignment is not in Assigned status";

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                assignment.Status = AssignmentStatus.Accepted;
                assignment.AcceptedAt = DateTime.UtcNow;
                await _assignmentRepo.UpdateAsync(assignment);

                assignment.Unit.Status = UnitStatus.EnRoute;
                await _unitRepo.UpdateAsync(assignment.Unit);

                assignment.Incident.Status = IncidentStatus.InProgress;
                assignment.Incident.UpdatedAt = DateTime.UtcNow;
                await _incidentRepo.UpdateAsync(assignment.Incident);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return "Assignment accepted successfully.";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ex.Message;
            }
        }

        // Reject Assignment
        public async Task<string> RejectAsync(int incidentId, int unitId, string reason)
        {
            var assignment = await _assignmentRepo.GetByCompositeKeyAsync(incidentId, unitId);
            if (assignment == null) return "Assignment not found";

            if (assignment.Status != AssignmentStatus.Assigned)
                return "Assignment cannot be rejected";

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                assignment.Status = AssignmentStatus.Rejected;
                assignment.RejectedAt = DateTime.UtcNow;
                assignment.RejectionReason = reason;
                await _assignmentRepo.UpdateAsync(assignment);

                assignment.Unit.Status = UnitStatus.Available;
                await _unitRepo.UpdateAsync(assignment.Unit);

                assignment.Incident.Status = IncidentStatus.WaitingForUnit;
                assignment.Incident.UpdatedAt = DateTime.UtcNow;
                await _incidentRepo.UpdateAsync(assignment.Incident);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return "Assignment rejected.";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ex.Message;
            }
        }

        // Arrive at Scene
        public async Task<string> ArriveAsync(int incidentId, int unitId)
        {
            var assignment = await _assignmentRepo.GetByCompositeKeyAsync(incidentId, unitId);
            if (assignment == null) return "Assignment not found";

            if (assignment.Status != AssignmentStatus.Accepted)
                return "Must accept assignment first";

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                assignment.ArrivedAt = DateTime.UtcNow;
                await _assignmentRepo.UpdateAsync(assignment);

                assignment.Unit.Status = UnitStatus.OnScene;
                await _unitRepo.UpdateAsync(assignment.Unit);

                assignment.Incident.Status = IncidentStatus.OnScene;
                assignment.Incident.UpdatedAt = DateTime.UtcNow;
                await _incidentRepo.UpdateAsync(assignment.Incident);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return "Arrived at scene.";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ex.Message;
            }
        }

        // Complete Assignment
        public async Task<string> CompleteAsync(int incidentId, int unitId, string notes)
        {
            var assignment = await _assignmentRepo.GetByCompositeKeyAsync(incidentId, unitId);
            if (assignment == null) return "Assignment not found";

            if (!assignment.ArrivedAt.HasValue)
                return "Must arrive first";

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                assignment.Status = AssignmentStatus.Completed;
                assignment.CompletedAt = DateTime.UtcNow;
                assignment.CompletionNotes = notes;
                await _assignmentRepo.UpdateAsync(assignment);

                assignment.Unit.Status = UnitStatus.Available;
                await _unitRepo.UpdateAsync(assignment.Unit);

                assignment.Incident.Status = IncidentStatus.Resolved;
                assignment.Incident.ResolvedAt = DateTime.UtcNow;
                assignment.Incident.UpdatedAt = DateTime.UtcNow;
                await _incidentRepo.UpdateAsync(assignment.Incident);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return "Assignment completed.";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ex.Message;
            }
        }
    }
}