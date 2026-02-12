using System.Text.Json.Serialization;

namespace SmartCity.Domain.Results
{
    public class AIDetectionResponse
    {
        [JsonPropertyName("detected")]
        public bool Detected { get; set; }

        [JsonPropertyName("confidence")]
        public float Confidence { get; set; }

        // ✅ This should match Python's response
        [JsonPropertyName("incident_type")]
        public string IncidentType { get; set; } = string.Empty;

        [JsonPropertyName("alert_level")]
        public string AlertLevel { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        // ✅ This is what we use for unit assignment
        [JsonPropertyName("recommended_unit_type")]
        public string RecommendedUnitType { get; set; } = string.Empty;

        [JsonPropertyName("coordinates")]
        public Dictionary<string, double>? Coordinates { get; set; }
    }
}