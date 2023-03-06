using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using MVCCaching.Base.Core.Interfaces;
using MVCCaching;
using Core;

namespace Account.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IRoleInfoProvider _roleInfoProvider;
        private readonly ISiteRepository _siteRepository;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly IUserInfoProvider _userInfoProvider;

        public RoleRepository(IRoleInfoProvider roleInfoProvider,
            ISiteRepository siteRepository,
            IProgressiveCache progressiveCache,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            IUserInfoProvider userInfoProvider)
        {
            _roleInfoProvider = roleInfoProvider;
            _siteRepository = siteRepository;
            _progressiveCache = progressiveCache;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _userInfoProvider = userInfoProvider;
        }

        public async Task<Result<RoleItem>> GetRoleAsync(string roleName, string siteName)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Object(RoleInfo.OBJECT_TYPE, roleName);

            var role = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _roleInfoProvider.GetAsync(roleName, await _siteRepository.GetSiteIDAsync(siteName));
            }, new CacheSettings(60, "GetRoleAsync", roleName, siteName));

            if (role != null)
            {
                return new RoleItem(
                    siteID: role.SiteID.ToObjectIdentity(),
                    roleID: role.RoleID,
                    roleDisplayName: role.RoleDisplayName,
                    roleName: role.RoleName,
                    roleGUID: role.RoleGUID
                    )
                {
                    RoleIsDomain = role.RoleIsDomain,
                    RoleDescription = role.RoleDescription.AsNullOrWhitespaceMaybe()
                };
            }
            else
            {
                return Result.Failure<RoleItem>("Could not find role");
            }
        }


        public async Task<bool> UserInRoleAsync(int userID, string roleName, string siteName)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.ObjectType(UserRoleInfo.OBJECT_TYPE);

            var roleItem = await GetRoleAsync(roleName, siteName);
            if (roleItem.TryGetValue(out var role))
            {
                return UserRoleInfoProvider.IsUserInRole(userID, role.RoleID);
            }
            return false;
        }

        public async Task<bool> UserHasPermissionAsync(int userID, string resourceName, string permissionName, string siteName)
        {
            var builder = _cacheDependencyBuilderFactory.Create()
                .ObjectType(UserRoleInfo.OBJECT_TYPE)
                .ObjectType(RolePermissionInfo.OBJECT_TYPE)
                .AddKey("cms.permission|all");

            // For next call, only cache on user id
            builder = _cacheDependencyBuilderFactory.Create()
                .Object(UserInfo.OBJECT_TYPE, userID);
            var user = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _userInfoProvider.GetAsync(userID);
            }, new CacheSettings(60, "GetUserByID", userID));

            return user != null && UserSecurityHelper.IsAuthorizedPerResource(resourceName, permissionName, siteName, user);
        }
    }
}