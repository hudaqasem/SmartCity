using AutoMapper;
using SmartCity.AppCore.Mapping.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.AppCore.Mapping.Devices
{
    public partial class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            GetDeviceListMapping();
        }
    }
}
