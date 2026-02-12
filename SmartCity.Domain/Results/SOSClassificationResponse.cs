using System.Text.Json.Serialization;

namespace SmartCity.Domain.Results
{
    /// <summary>
    /// Response from AI text classification (SOS detection)
    /// Matches Python FastAPI SOSClassificationResponse model
    /// </summary>
    public class SOSClassificationResponse
    {
        [JsonPropertyName("is_emergency")]
        public bool IsEmergency { get; set; }

        [JsonPropertyName("confidence")]
        public float Confidence { get; set; }

        [JsonPropertyName("incident_type")]
        public string IncidentType { get; set; } = string.Empty;

        [JsonPropertyName("alert_level")]
        public string AlertLevel { get; set; } = string.Empty;

        [JsonPropertyName("keywords_detected")]
        public List<string> KeywordsDetected { get; set; } = new();

        [JsonPropertyName("recommended_unit_type")]
        public string RecommendedUnitType { get; set; } = string.Empty;
    }

}
