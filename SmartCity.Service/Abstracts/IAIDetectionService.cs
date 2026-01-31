using SmartCity.Domain.Results;

namespace SmartCity.Service.Abstracts
{
    /// <summary>
    /// Service for AI-powered incident detection using Python AI models
    /// Communicates with Python FastAPI service for fire, accident, and SOS text detection
    /// </summary>
    public interface IAIDetectionService
    {
        /// <summary>
        /// Detect fire in an image using the fire.pt model
        /// </summary>
        /// <param name="imageData">Image bytes (JPEG/PNG)</param>
        /// <returns>Detection result with confidence and alert level</returns>
        Task<AIDetectionResponse> DetectFireAsync(byte[] imageData);

        /// <summary>
        /// Detect accident in an image using the accident.pt model
        /// </summary>
        /// <param name="imageData">Image bytes (JPEG/PNG)</param>
        /// <returns>Detection result with confidence and alert level</returns>
        Task<AIDetectionResponse> DetectAccidentAsync(byte[] imageData);

        /// <summary>
        /// Classify text for SOS/emergency keywords using HuggingFace model
        /// </summary>
        /// <param name="text">Text to analyze</param>
        /// <param name="latitude">Optional location latitude</param>
        /// <param name="longitude">Optional location longitude</param>
        /// <returns>Classification result with emergency flag and keywords</returns>
        Task<SOSClassificationResponse> ClassifyTextAsync(
            string text,
            double? latitude = null,
            double? longitude = null);

        /// <summary>
        /// Check if Python AI service is running and healthy
        /// </summary>
        /// <returns>True if service is responsive</returns>
        Task<bool> IsAIServiceHealthyAsync();
    }
}