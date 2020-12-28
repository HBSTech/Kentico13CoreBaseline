using CMS.Membership;
using MVCCaching;
using Generic.Repositories.Interfaces;
using Generic.Repositories.Helpers.Interfaces;
using CMS.Core;

namespace Generic.Repositories.Implementations
{
    public class KenticoRoleRepository : IRoleRepository
    {
        private IKenticoRoleRepositoryHelper _Helper;
        private IUserRoleInfoProvider _userRoleInfoProvider;
        private IEventLogService _eventLogService;

        public KenticoRoleRepository(IKenticoRoleRepositoryHelper Helper,
            IUserRoleInfoProvider userRoleInfoProvider,
            IEventLogService eventLogService)
        {
            _Helper = Helper;
            _userRoleInfoProvider = userRoleInfoProvider;
            _eventLogService = eventLogService;
        }

        [DoNotCache]
        public RoleInfo GetRole(string RoleName, string SiteName, string[] Columns = null)
        {
            return _Helper.GetRole(RoleName, SiteName, Columns);
        }

        [DoNotCache]
        public void SetUserRole(int UserID, string RoleName, string SiteName, bool RoleToggle)
        {
            var Role = _Helper.GetRole(RoleName, SiteName, new string[] { "RoleID" });
            if (RoleToggle)
            {
                if (_userRoleInfoProvider.Get(UserID, Role.RoleID) == null)
                {
                    _userRoleInfoProvider.Add(UserID, Role.RoleID);
                }
            }
            else
            {
                var ExistingUserRole = _userRoleInfoProvider.Get(UserID, Role.RoleID);
                if (ExistingUserRole != null)
                {
                    ExistingUserRole.Delete();
                }
            }
        }

        [CacheDependency("cms.userrole|all")]
        public bool UserInRole(int UserID, string RoleName, string SiteName)
        {
            var Role = _Helper.GetRole(RoleName, SiteName, new string[] { "RoleID" });
            if (Role == null)
            {
                _eventLogService.LogEvent(EventTypeEnum.Error, "KenticoUserRepository", "NoAdministratorRole", "No Administrator Role found! Please add!");
            }
            return UserRoleInfoProvider.IsUserInRole(UserID, Role.RoleID);
        }

        [CacheDependency("cms.user|byid|{0}")]
        [CacheDependency("cms.userrole|all")]
        [CacheDependency("cms.permission|all")]
        [CacheDependency("cms.rolepermission|all")]
        public bool UserHasPermission(int UserID, string ResourceName, string PermissionName, string SiteName)
        {
            return UserSecurityHelper.IsAuthorizedPerResource(ResourceName, PermissionName, SiteName, _Helper.GetUser(UserID));
        }

        
    }
}