using CMS.Membership;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using System.Threading.Tasks;

namespace Generic.Services.Implementation
{
    public class RoleService : IRoleService
    {
        readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task SetUserRole(int userID, string roleName, string siteName, bool roleToggle)
        {
            var Role = await _roleRepository.GetRoleAsync(roleName, siteName);
            if (roleToggle && !(await _roleRepository.UserInRoleAsync(userID, roleName, siteName)))
            {
                UserInfoProvider.AddUserToRole(userID, Role.RoleID);
            }
            else
            {
                UserInfoProvider.RemoveUserFromRole(userID, Role.RoleID);
            }
        }
    }
}