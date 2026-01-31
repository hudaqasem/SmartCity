namespace SmartCity.AppCore.Features.Incidents.Commands.Results
{
    public class AssignmentInfo
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitContact { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}
