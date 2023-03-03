using CMS.Membership;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using System.Threading.Tasks;

namespace Generic.Services.Implementation
{
    [AutoDependencyInjection]
    public class RoleService : IRoleService
    {
        readonly IRoleRepository _roleRepository;
        private readonly IRoleInfoProvider _roleInfoProvider;
        private readonly ISiteRepository _siteRepository;

        public RoleService(IRoleRepository roleRepository,
            IRoleInfoProvider roleInfoProvider,
            ISiteRepository siteRepository)
        {
            _roleRepository = roleRepository;
            _roleInfoProvider = roleInfoProvider;
            _siteRepository = siteRepository;
        }

        public async Task CreateRoleIfNotExisting(string roleName, string siteName)
        {
            var role = await _roleRepository.GetRoleAsync(roleName, siteName);
            if(role == null)
            {
                var newRole = new RoleInfo()
                {
                    RoleName = roleName,
                    RoleDisplayName = roleName,
                    SiteID = await _siteRepository.GetSiteIDAsync(siteName),
                    RoleDescription = "auto generated from IAuthenticationConfiguration settings"
                };
                _roleInfoProvider.Set(newRole);
            }
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