using SmartCity.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCity.Domain.Models
{
    public class Assignment
    {
        public int IncidentId { get; set; }
        public Incident? Incident { get; set; }
        public int UnitId { get; set; }
        public ResponseUnit? Unit { get; set; }
        public AssignmentStatus Status { get; set; } // Pending, Assigned, Accepted, Completed
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ArrivedAt { get; set; }
        public string? CompletionNotes { get; set; }

        //public int? DispatcherUserId { get; set; }
        //public ApplicationUser? DispatcherUser { get; set; }


        [NotMapped]
        public TimeSpan? ResponseTime =>
            AcceptedAt.HasValue ? AcceptedAt.Value - AssignedAt : null;

        [NotMapped]
        public TimeSpan? CompletionTime =>
            CompletedAt.HasValue
                ? CompletedAt.Value - (ArrivedAt ?? AcceptedAt ?? AssignedAt)
                : null;
    }
}
