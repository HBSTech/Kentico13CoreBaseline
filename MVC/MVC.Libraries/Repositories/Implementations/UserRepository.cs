using CMS.Membership;
using System;
using Generic.Repositories.Interfaces;
using Generic.Models;
using Generic.Libraries.Helpers;
using System.Threading.Tasks;
using MVCCaching.Base.Core.Interfaces;
using CMS.Helpers;
using System.Reflection;
using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Generic.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserInfoProvider _userInfoProvider;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteRepository _siteRepository;
        private readonly IProgressiveCache _progressiveCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserRepository(IUserInfoProvider userInfoProvider,
            ICacheDependenciesStore cacheDependenciesStore,
            ISiteRepository siteRepository,
            IProgressiveCache progressiveCache,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _userInfoProvider = userInfoProvider;
            _cacheDependenciesStore = cacheDependenciesStore;
            _siteRepository = siteRepository;
            _progressiveCache = progressiveCache;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var username = _httpContextAccessor.HttpContext.User?.Identity?.Name ?? "public";
            return await GetUserAsync(username);
        }

        public async Task<User> GetUserAsync(int userID)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(UserInfo.OBJECT_TYPE, userID);

            var user = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                var user = await _userInfoProvider.GetAsync(userID);
                user ??= await _userInfoProvider.GetAsync("public");
                return user;
            }, new CacheSettings(15, MethodBase.GetCurrentMethod(), userID));
            return user != null ? _mapper.Map<User>(user) : null;
        }

        public async Task<User> GetUserAsync(string userName)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(UserInfo.OBJECT_TYPE, userName);

            var user = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                var user = await _userInfoProvider.GetAsync(userName);
                user ??= await _userInfoProvider.GetAsync("public");
                return user;
            }, new CacheSettings(15, MethodBase.GetCurrentMethod(), userName));
            return user != null ? _mapper.Map<User>(user) : null;
        }

        public async Task<User> GetUserAsync(Guid userGuid)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(UserInfo.OBJECT_TYPE, userGuid);

            var user = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                var user = await _userInfoProvider.GetAsync(userGuid);
                user ??= await _userInfoProvider.GetAsync("public");
                return user;
            }, new CacheSettings(15, MethodBase.GetCurrentMethod(), userGuid));
            return user != null ? _mapper.Map<User>(user) : null;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.ObjectType(UserInfo.OBJECT_TYPE);

            var user = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                var user = (await _userInfoProvider.Get()
                .WhereEquals(nameof(UserInfo.Email), email)
                .TopN(1)
                .GetEnumerableTypedResultAsync()).FirstOrDefault();
                user ??= await _userInfoProvider.GetAsync("public");
                return user;
            }, new CacheSettings(15, MethodBase.GetCurrentMethod(), email));
            return user != null ? _mapper.Map<User>(user) : null;
        }
    }
}