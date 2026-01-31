namespace SmartCity.Domain.Results
{
    /// <summary>
    /// Response from AI image detection (fire/accident)
    /// Matches Python FastAPI DetectionResponse model
    /// </summary>
    public class AIDetectionResponse
    {
        /// <summary>
        /// Whether the incident was detected in the image
        /// </summary>
        public bool Detected { get; set; }

        /// <summary>
        /// Confidence score (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// Type of incident detected (Fire, Medical, Police)
        /// </summary>
        public string IncidentType { get; set; } = string.Empty;

        /// <summary>
        /// Alert severity level (HIGH, MEDIUM, LOW)
        /// </summary>
        public string AlertLevel { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable description of the detection
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Recommended response unit type for this incident
        /// </summary>
        public string RecommendedUnitType { get; set; } = string.Empty;

        /// <summary>
        /// Optional coordinates if detected
        /// </summary>
        public Dictionary<string, double>? Coordinates { get; set; }
    }



}