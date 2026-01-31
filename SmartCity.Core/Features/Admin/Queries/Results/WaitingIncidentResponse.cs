namespace SmartCity.AppCore.Features.Admin.Queries.Results
{
    public class WaitingIncidentResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan WaitingDuration => DateTime.UtcNow - CreatedAt;
        public string ReportedBy { get; set; }
    }
}
