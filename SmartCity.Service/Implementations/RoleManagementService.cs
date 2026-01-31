using Microsoft.AspNetCore.Identity;
using SmartCity.Domain.Models;
using SmartCity.Service.Abstracts;

namespace SmartCity.Service.Implementations
{
    public class RoleManagementService : IRoleManagementService
    {
        #region Fields
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        // System roles that can not be modified or deleted
        private readonly string[] _systemRoles = { "Admin", "Citizen", "ResponseUnit" };

        #endregion

        #region Constructors
        public RoleManagementService(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager
                                   )
        {
            _roleManager = roleManager;
            _userManager = userManager;

        }


        #endregion

        #region Handle Functions
        public async Task<bool> IsRoleExistByName(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        #region Add role
        public async Task<string> AddRoleAsync(string roleName)
        {
            var role = new IdentityRole()
            {
                Name = roleName
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
                return "Success";
            return "Failed";
        }

        #endregion

        #region Edit Role
        public async Task<string> EditRoleAsync(string id, string newName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return "NotFound";

            if (_systemRoles.Contains(role.Name))
                return "CannotEditSystemRole";

            role.Name = newName;

            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded ? "Updated" : "Failed";
        }

        #endregion

        #region Delete Role

        public async Task<string> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return "NotFound";

            //  prevent deleting system roles
            if (_systemRoles.Contains(role.Name))
                return "CannotDeleteSystemRole";

            // check if role has users assigned
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
                return $"RoleHasUsers:{usersInRole.Count}";

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded ? "Deleted" : "Failed";
        }

        #endregion

        #region Get & Get by id
        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            return _roleManager.Roles.ToList();
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        #endregion


        #endregion
    }
}
