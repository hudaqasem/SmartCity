using SmartCity.Domain.Models;

namespace SmartCity.Service.Abstracts
{
    public interface IResponseUnitService
    {
        Task<List<ResponseUnit>> GetResponseUnitsAsync();
        Task<ResponseUnit> GetResponseUnitByUserIdAsync(string userId);
        Task<ResponseUnit> GetResponseUnitByIdAsync(int unitId);
    }
}
