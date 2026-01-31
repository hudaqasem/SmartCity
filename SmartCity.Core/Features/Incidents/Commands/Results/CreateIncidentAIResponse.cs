using SmartCity.Domain.Results;

namespace SmartCity.AppCore.Features.Incidents.Commands.Results
{
    public class CreateIncidentAIResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public AssignmentInfo? Assignment { get; set; }

        /// <summary>
        /// AI Detection metadata (if AI was used)
        /// </summary>
        public AIDetectionInfo? AIDetection { get; set; }
    }
}
