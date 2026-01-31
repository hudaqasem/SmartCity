using SmartCity.Domain.Enums;

namespace SmartCity.Domain.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public IncidentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }


        //*************** Relations

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? ReportedByUserId { get; set; }
        public ApplicationUser? ReportedByUser { get; set; }

        public int? ReportedByDeviceId { get; set; }
        public Device? ReportedByDevice { get; set; }

        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<Media>? MediaFiles { get; set; }
        public ICollection<Assignment>? Assignments { get; set; }

    }
}
