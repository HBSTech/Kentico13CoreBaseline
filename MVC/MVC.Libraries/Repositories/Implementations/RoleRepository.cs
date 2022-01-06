using CMS.Membership;
using Generic.Repositories.Interfaces;
using Generic.Libraries.Helpers;
using Generic.Models;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Helpers;
using System.Reflection;
using MVCCaching.Base.Core.Interfaces;
using CMS.Modules;

namespace Generic.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IRoleInfoProvider _roleInfoProvider;
        private readonly ISiteRepository _siteRepository;
        private readonly IMapper _mapper;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly IUserInfoProvider _userInfoProvider;

        public RoleRepository(IRoleInfoProvider roleInfoProvider,
            ISiteRepository siteRepository,
            IMapper mapper,
            IProgressiveCache progressiveCache,
            ICacheDependenciesStore cacheDependenciesStore,
            IUserInfoProvider userInfoProvider)
        {
            _roleInfoProvider = roleInfoProvider;
            _siteRepository = siteRepository;
            _mapper = mapper;
            _progressiveCache = progressiveCache;
            _cacheDependenciesStore = cacheDependenciesStore;
            _userInfoProvider = userInfoProvider;
        }

        public async Task<RoleItem> GetRoleAsync(string roleName, string siteName)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(RoleInfo.OBJECT_TYPE, roleName);

            var role = await _progressiveCache.LoadAsync(async cs =>
            {
                if(cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _roleInfoProvider.GetAsync(roleName, await _siteRepository.GetSiteIDAsync(siteName));
            }, new CacheSettings(60, "GetRoleAsync", roleName, siteName));

            if(role != null)
            {
                return _mapper.Map<RoleItem>(role);
            }else
            {
                return null;
            }
        }


        public async Task<bool> UserInRoleAsync(int userID, string roleName, string siteName)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.ObjectType(UserRoleInfo.OBJECT_TYPE);

            var roleItem = await GetRoleAsync(roleName, siteName);           
            return UserRoleInfoProvider.IsUserInRole(userID, roleItem.RoleID);
        }

        public async Task<bool> UserHasPermissionAsync(int userID, string resourceName, string permissionName, string siteName)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.ObjectType(UserRoleInfo.OBJECT_TYPE)
                .ObjectType(RolePermissionInfo.OBJECT_TYPE)
                .CustomKey("cms.permission|all");

            // For next call, only cache on user id
            builder.Clear()
                .Object(UserInfo.OBJECT_TYPE, userID);
            var user = await _progressiveCache.LoadAsync(async cs =>
            {
                if(cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _userInfoProvider.GetAsync(userID);
            }, new CacheSettings(60, "GetUserByID", userID));

            return user != null && UserSecurityHelper.IsAuthorizedPerResource(resourceName, permissionName, siteName, user);
        }
    }
}