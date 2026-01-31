using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Infrastructure.Repositories
{
    public class DeviceRepository : GenericRepositoryAsync<Device> , IDeviceRepository
    {
        #region Fields

        private readonly DbSet<Device> device;

        #endregion

        #region Constructors
        public DeviceRepository(ApplicationDbContext _context) : base(_context)
        {
            device = _context.Set<Device>();
        }

        #endregion

        #region Handles Functions

        #endregion
    }
}
