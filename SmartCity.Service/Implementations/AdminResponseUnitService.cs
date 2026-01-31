using Microsoft.AspNetCore.Identity;
using SmartCity.Domain.Enums;
using SmartCity.Domain.Models;
using SmartCity.Infrastructure.Abstracts;
using SmartCity.Service.Abstracts;

namespace SmartCity.Service.Implementations
{
    public class AdminResponseUnitService : IAdminResponseUnitService
    {
        #region Fields

        private readonly IResponseUnitRepository _responseUnitRepository;

        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Constructors
        public AdminResponseUnitService(IResponseUnitRepository responseUnitRepository,
           UserManager<ApplicationUser> userManager)
        {
            _responseUnitRepository = responseUnitRepository;
            _userManager = userManager;
        }



        #endregion

        #region Handles Functions
        public async Task<string> AddAsync(ResponseUnit unit)
        {
            await _responseUnitRepository.AddAsync(unit);
            return "Success";
        }
        public async Task<string> EditAsync(ResponseUnit unit)
        {
            await _responseUnitRepository.UpdateAsync(unit);
            return "Success";
        }

        public async Task<string> AssignUserAsync(int unitId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (!await _userManager.IsInRoleAsync(user, "ResponseUnit"))
                return "User must have ResponseUnit role";

            var existingUnit = await _responseUnitRepository.GetByUserIdAsync(userId);
            if (existingUnit != null)
                return "User already assigned to another unit";

            var unit = await _responseUnitRepository.GetByIdAsync(unitId);


            unit.UserId = userId;

            await _responseUnitRepository.UpdateAsync(unit);

            return "User linked successfully";
        }

        public async Task<string> ChangeStatusAsync(int unitId, UnitStatus status)
        {
            var unit = await _responseUnitRepository.GetByIdAsync(unitId);
            unit.Status = status;

            await _responseUnitRepository.UpdateAsync(unit);
            return "Unit status updated";
        }


        public async Task<string> ToggleActiveAsync(int unitId, bool isActive)
        {
            var unit = await _responseUnitRepository.GetByIdAsync(unitId);
            unit.IsActive = isActive;

            await _responseUnitRepository.UpdateAsync(unit);
            return "Unit state updated";
        }


        #endregion
    }
}
