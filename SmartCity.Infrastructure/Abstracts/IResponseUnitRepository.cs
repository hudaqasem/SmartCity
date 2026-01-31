using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Generics;

namespace SmartCity.Infrastructure.Abstracts
{
    public interface IResponseUnitRepository : IGenericRepositoryAsync<ResponseUnit>
    {
        Task<List<ResponseUnit>> GetAvailableUnitsByTypeAsync(string type);
        Task<List<ResponseUnit>> GetByTypeAsync(string type);
        Task<List<ResponseUnit>> GetAvailableUnitsAsync();
        Task<ResponseUnit?> GetByUserIdAsync(string userId);
    }
}
