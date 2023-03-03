using CMS.Base;
using CMS.Helpers;
using CMS.SiteProvider;
using Generic.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class SiteRepository : ISiteRepository
    {
        private readonly ISiteInfoProvider _siteInfoProvider;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ISiteService _siteService;

        public SiteRepository(ISiteService siteService,
            ISiteInfoProvider siteInfoProvider,
            IProgressiveCache progressiveCache)
        {
            _siteInfoProvider = siteInfoProvider;
            _progressiveCache = progressiveCache;
            _siteService = siteService;
        }
        public string CurrentSiteName()
        {
            return _siteService.CurrentSite.SiteName;
        }

        public Task<string> CurrentSiteNameAsync()
        {
            return Task.FromResult(_siteService.CurrentSite.SiteName);
        }

        public async Task<int> GetSiteIDAsync(string siteName = null)
        {
            if(string.IsNullOrWhiteSpace(siteName) || _siteService.CurrentSite.SiteName.Equals(siteName, StringComparison.InvariantCultureIgnoreCase))
            {
                return SiteContext.CurrentSiteID;
            } else
            {
                var siteID = await _progressiveCache.LoadAsync(async cs =>
                {
                    if (cs.Cached)
                    {
                        cs.CacheDependency = CacheHelper.GetCacheDependency($"{SiteInfo.OBJECT_TYPE}|byname|{siteName}");
                    }
                    return (await _siteInfoProvider.GetAsync(siteName))?.SiteID ?? 0;
                }, new CacheSettings(1440, "GetSiteID", siteName));
                return siteID;
            }
        }
    }
}
