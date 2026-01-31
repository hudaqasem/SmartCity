using SmartCity.AppCore.Features.Devices.Queries.Results;
using SmartCity.AppCore.Features.ResponseUnits.Queries.Results;
using SmartCity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.AppCore.Mapping.Devices
{
    public partial class DeviceProfile
    {
        public void GetDeviceListMapping()
        {
            CreateMap<Device, GetDeviceListResponse>();
        }
    }
}
