using CMS.DataEngine;
using CMS.Membership;
using Generic.Library.Helpers;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;
using System.Linq;

namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoRoleRepositoryHelper : IKenticoRoleRepositoryHelper
    {
        private IUserInfoProvider _userInfoProvider;
        private IRoleInfoProvider _roleInfoProvider;

        public KenticoRoleRepositoryHelper(IUserInfoProvider userInfoProvider, IRoleInfoProvider roleInfoProvider)
        {
            _userInfoProvider = userInfoProvider;
            _roleInfoProvider = roleInfoProvider;
        }

        [CacheDependency("cms.user|byid|{0}")]
        public UserInfo GetUser(int UserID)
        {
            return _userInfoProvider.Get(UserID);
        }

        [CacheDependency("cms.role|byname|{0}")]
        public RoleInfo GetRole(string RoleName, string SiteName, string[] Columns = null)
        {
            return _roleInfoProvider.Get()
                .OnSite(new SiteInfoIdentifier(SiteName))
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }
    }
}