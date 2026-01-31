using SmartCity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Service.Abstracts
{
    public interface IDeviceService
    {
        Task<List<Device>> GetDevicesAsync();
    }
}
