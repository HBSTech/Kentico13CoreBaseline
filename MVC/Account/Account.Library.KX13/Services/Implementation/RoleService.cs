using CMS.Membership;
using MVCCaching;

namespace Account.Services.Implementation
{
    [AutoDependencyInjection]
    public class RoleService : IRoleService
    {
        readonly IRoleRepository _roleRepository;
        private readonly IRoleInfoProvider _roleInfoProvider;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRoleInfoProvider _userRoleInfoProvider;

        public RoleService(IRoleRepository roleRepository,
            IRoleInfoProvider roleInfoProvider,
            ISiteRepository siteRepository,
            IUserRoleInfoProvider userRoleInfoProvider)
        {
            _roleRepository = roleRepository;
            _roleInfoProvider = roleInfoProvider;
            _siteRepository = siteRepository;
            _userRoleInfoProvider = userRoleInfoProvider;
        }

        public async Task CreateRoleIfNotExisting(string roleName, string siteName)
        {
            var roleResult = await _roleRepository.GetRoleAsync(roleName, siteName);
            if (roleResult.TryGetValue(out var role))
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
            return;
        }

        public async Task SetUserRole(int userID, string roleName, string siteName, bool roleToggle)
        {
            var roleResult = await _roleRepository.GetRoleAsync(roleName, siteName);
            if (roleResult.TryGetValue(out var role))
            {
                if (roleToggle && !(await _roleRepository.UserInRoleAsync(userID, roleName, siteName)))
                {
                    _userRoleInfoProvider.Add(userID, role.RoleID);
                }
                else
                {
                    _userRoleInfoProvider.Remove(userID, role.RoleID);
                }
            }
            return;
        }
    }
}