using SmartCity.Domain.Enums;

namespace SmartCity.AppCore.Features.ResponseUnits.Queries.Results
{
    public class UnitAssignmentResponse
    {
        public int AssignmentId { get; set; }
        public int IncidentId { get; set; }
        public string IncidentType { get; set; }
        public string IncidentDescription { get; set; }
        public double? IncidentLatitude { get; set; }
        public double? IncidentLongitude { get; set; }
        public string IncidentLocation { get; set; }
        public AssignmentStatus Status { get; set; }
        public IncidentStatus IncidentStatus { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan? ResponseTime => AcceptedAt.HasValue
            ? AcceptedAt.Value - AssignedAt
            : null;
        public TimeSpan? TotalTime => CompletedAt.HasValue
            ? CompletedAt.Value - AssignedAt
            : null;
        public string ReportedBy { get; set; }
    }
}
