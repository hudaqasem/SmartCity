using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.AppCore.Features.Devices.Queries.Results
{
    public class GetDeviceListResponse
    {
        public int Id { get; set; }
        public string DeviceIdentifier { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastSeen { get; set; }
    }
}
