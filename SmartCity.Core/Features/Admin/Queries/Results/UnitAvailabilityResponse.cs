using SmartCity.Domain.Enums;

namespace SmartCity.AppCore.Features.Admin.Queries.Results
{
    public class UnitAvailabilityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public UnitStatus Status { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; }
        public string Contact { get; set; }
    }
}
