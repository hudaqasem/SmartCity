using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Service.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Service.Implementations
{
    public class DeviceService : IDeviceService
    {
        #region Fields

        private readonly IDeviceRepository deviceRepository;

        #endregion

        #region Constructors
        public DeviceService(IDeviceRepository deviceRepo)
        {
            deviceRepository = deviceRepo;
        }

        #endregion

        #region Handles Functions

        public async Task<List<Device>> GetDevicesAsync()
        {
            return await deviceRepository.GetAllAsync();
        }

        #endregion
    
    }
}
