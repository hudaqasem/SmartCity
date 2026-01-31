namespace SmartCity.AppCore.Features.Incidents.Queries.Results
{
    public class GetIncidentListResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
