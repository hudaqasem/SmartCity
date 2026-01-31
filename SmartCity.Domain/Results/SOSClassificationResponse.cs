namespace SmartCity.Domain.Results
{
    /// <summary>
    /// Response from AI text classification (SOS detection)
    /// Matches Python FastAPI SOSClassificationResponse model
    /// </summary>
    public class SOSClassificationResponse
    {
        /// <summary>
        /// Whether the text indicates an emergency
        /// </summary>
        public bool IsEmergency { get; set; }

        /// <summary>
        /// Confidence score (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// Type of incident detected from text
        /// </summary>
        public string IncidentType { get; set; } = string.Empty;

        /// <summary>
        /// Alert severity level (HIGH, MEDIUM, LOW)
        /// </summary>
        public string AlertLevel { get; set; } = string.Empty;

        /// <summary>
        /// Emergency keywords found in the text
        /// </summary>
        public List<string> KeywordsDetected { get; set; } = new();

        /// <summary>
        /// Recommended response unit type based on text analysis
        /// </summary>
        public string RecommendedUnitType { get; set; } = string.Empty;
    }

}
