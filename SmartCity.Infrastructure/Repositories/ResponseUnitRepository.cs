using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Repositories
{
    public class ResponseUnitRepository : GenericRepositoryAsync<ResponseUnit>,
        IResponseUnitRepository
    {
        private readonly DbSet<ResponseUnit> _units;

        public ResponseUnitRepository(ApplicationDbContext context) : base(context)
        {
            _units = context.Set<ResponseUnit>();
        }

        public async Task<List<ResponseUnit>> GetAvailableUnitsByTypeAsync(string type)
        {
            return await _units
                .Where(u => u.Type == type &&
                           u.Status == UnitStatus.Available &&
                           u.IsActive &&
                           u.Latitude.HasValue &&
                           u.Longitude.HasValue)
                .ToListAsync();
        }

        public async Task<List<ResponseUnit>> GetByTypeAsync(string type)
        {
            return await _units
                .Where(u => u.Type == type &&
                           u.IsActive &&
                           u.Latitude.HasValue &&
                           u.Longitude.HasValue)
                .ToListAsync();
        }

        public async Task<List<ResponseUnit>> GetAvailableUnitsAsync()
        {
            return await _units
                .Where(u => u.Status == UnitStatus.Available &&
                           u.IsActive)
                .ToListAsync();
        }

        public async Task<ResponseUnit?> GetByUserIdAsync(string userId)
        {
            return await _units
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
        }


    }
}
