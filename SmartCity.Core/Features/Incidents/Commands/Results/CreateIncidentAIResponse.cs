using SmartCity.Domain.Results;

namespace SmartCity.AppCore.Features.Incidents.Commands.Results
{
    public class CreateIncidentAIResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Assignment details (if unit was assigned)
        /// </summary>
        public AssignmentInfo? Assignment { get; set; }

        /// <summary>
        /// AI detection metadata (only present if AI detection was used)
        /// </summary>
        public AIDetectionInfo? AIDetection { get; set; }
    }
}
