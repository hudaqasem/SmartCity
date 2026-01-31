using SmartCity.Domain.Enums;

namespace SmartCity.Domain.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Channel { get; set; }  // Email, SMS, Push ,signalR
        public string Payload { get; set; }
        public NotificationStatus Status { get; set; }
        public int RetryCount { get; set; } = 0;
        public DateTime? SentAt { get; set; }

        //*************** Relations

        public string? TargetUserId { get; set; }
        public ApplicationUser? TargetUser { get; set; }

        public int? IncidentId { get; set; }
        public Incident? Incident { get; set; }

    }
}
