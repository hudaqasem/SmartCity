using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;

namespace SmartCity.Service.Abstracts
{
    public interface IAdminResponseUnitService
    {
        Task<string> AddAsync(ResponseUnit unit);

        Task<string> EditAsync(ResponseUnit unit);
        Task<string> ChangeStatusAsync(int unitId, UnitStatus status);
        Task<string> ToggleActiveAsync(int unitId, bool isActive);

        Task<string> AssignUserAsync(int unitId, string userId);
    }
}
