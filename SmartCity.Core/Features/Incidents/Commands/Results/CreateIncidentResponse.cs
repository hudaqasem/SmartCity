namespace SmartCity.AppCore.Features.Incidents.Commands.Results
{
    public class CreateIncidentResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public AssignmentInfo? Assignment { get; set; }
        public DateTime CreatedAt { get; set; }
    }


}
