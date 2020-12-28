using CMS.Membership;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using MVCCaching;

namespace Generic.Services.Implementation
{
    public class KenticoRoleService : IRoleService
    {
        readonly IRoleRepository _RoleRepo;

        public KenticoRoleService(IRoleRepository RoleRepo)
        {
            _RoleRepo = RoleRepo;
        }

        public void SetUserRole(int UserID, string RoleName, string SiteName, bool RoleToggle)
        {
            var Role = _RoleRepo.GetRole(RoleName, SiteName, new string[] { "RoleID" });
            if (RoleToggle && !_RoleRepo.UserInRole(UserID, RoleName, SiteName))
            {
                UserInfoProvider.AddUserToRole(UserID, Role.RoleID);
            }
            else
            {
                UserInfoProvider.RemoveUserFromRole(UserID, Role.RoleID);
            }
        }
    }
}