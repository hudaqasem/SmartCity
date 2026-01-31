using SmartCity.Domain.Enums;

namespace SmartCity.AppCore.Features.Admin.Queries.Results
{
    public class NearestUnitResponse
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string Type { get; set; }
        public UnitStatus Status { get; set; }
        public double Distance { get; set; }
        public bool IsAvailable => Status == UnitStatus.Available;
        public string Contact { get; set; }
    }
}
