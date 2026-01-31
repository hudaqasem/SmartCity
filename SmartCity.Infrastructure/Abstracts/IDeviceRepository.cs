using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Infrastructure.Abstracts
{
    public interface IDeviceRepository : IGenericRepositoryAsync<Device>
    {
    }
}
