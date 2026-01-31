using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Commands.Results;

namespace SmartCity.AppCore.Features.Incidents.Commands.Models
{
    /// <summary>
    /// Command to create an incident with AI-powered detection
    /// Supports image analysis and text classification
    /// </summary>
    public class CreateIncidentWithAICommand : IRequest<Response<CreateIncidentAIResponse>>
    {
        /// <summary>
        /// Type of incident (Fire, Medical, Police) - can be overridden by AI
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Incident description - will be analyzed by AI text classifier
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Incident location latitude
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Incident location longitude
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// User ID who reported the incident
        /// </summary>
        public string? ReportedByUserId { get; set; }

        /// <summary>
        /// Image data for AI analysis (fire/accident detection)
        /// </summary>
        public byte[]? ImageData { get; set; }

        /// <summary>
        /// Enable AI detection and classification
        /// Set to false to skip AI processing
        /// </summary>
        public bool EnableAIDetection { get; set; } = true;

        /// <summary>
        /// Minimum confidence threshold for AI detection (0.0 to 1.0)
        /// Incidents below this threshold will be rejected
        /// </summary>
        public float MinimumConfidence { get; set; } = 0.5f;
    }
}