using Microsoft.AspNetCore.Identity;

namespace SmartCity.Service.Abstracts
{
    public interface IRoleManagementService
    {
        public Task<string> AddRoleAsync(string roleName);
        public Task<bool> IsRoleExistByName(string roleName);

        Task<string> EditRoleAsync(string id, string newName);
        Task<string> DeleteRoleAsync(string id);
        Task<List<IdentityRole>> GetRolesAsync();
        Task<IdentityRole?> GetRoleByIdAsync(string id);
    }
}
