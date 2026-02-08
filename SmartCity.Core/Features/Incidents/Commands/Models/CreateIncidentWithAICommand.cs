using MediatR;
using SmartCity.AppCore.Bases;
using SmartCity.AppCore.Features.Incidents.Commands.Results;

namespace SmartCity.AppCore.Features.Incidents.Commands.Models
{
    /// <summary>
    /// Command to create an incident with AI-powered detection
    /// Supports image analysis and text classification
    /// Type is OPTIONAL - AI will determine it automatically
    /// </summary>
    public class CreateIncidentWithAICommand : IRequest<Response<CreateIncidentAIResponse>>
    {
        /// <summary>
        /// OPTIONAL: Suggested incident type (Fire, Medical, Police)
        /// If provided but incorrect, AI will override it
        /// If not provided, AI will determine from image/text
        /// </summary>
        public string? Type { get; set; }

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
        /// Image data for AI analysis
        /// AI will run BOTH fire.pt AND accident.pt models
        /// and select the one with highest confidence
        /// </summary>
        public byte[]? ImageData { get; set; }

        /// <summary>
        /// Enable AI detection and classification
        /// Set to false to skip AI processing (acts like manual report)
        /// </summary>
        public bool EnableAIDetection { get; set; } = true;

        /// <summary>
        /// Minimum confidence threshold for AI detection (0.0 to 1.0)
        /// Incidents below this threshold will be rejected
        /// Default: 0.5 (50%)
        /// Recommended: 0.7 (70%) for production
        /// </summary>
        public float MinimumConfidence { get; set; } = 0.5f;
    }
}