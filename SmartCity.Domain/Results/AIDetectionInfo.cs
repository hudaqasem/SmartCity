namespace SmartCity.Domain.Results
{
    /// <summary>
    /// AI detection metadata to include in incident response
    /// </summary>
    public class AIDetectionInfo
    {
        public bool Detected { get; set; }
        public float Confidence { get; set; }
        public string AlertLevel { get; set; } = string.Empty;
        public string? ModelUsed { get; set; }
    }
}
