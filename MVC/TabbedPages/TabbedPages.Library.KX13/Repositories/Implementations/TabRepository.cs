using CMS.Base;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using CMS.Helpers;
using Core.Enums;
using Core.Models;
using Kentico.Content.Web.Mvc;
using Microsoft.SqlServer.Dac.Model;
using MVCCaching.Base.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabbedPages.Models;
using XperienceCommunity.QueryExtensions.Documents;

namespace TabbedPages.Repositories.Implementations
{
    public class TabRepository : ITabRepository
    {
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;
        private readonly IProgressiveCache _progressiveCache;
        private readonly IIdentityService _identityService;
        private readonly ISiteService _siteService;
        private readonly ISiteRepository _siteRepository;

        public TabRepository(ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ICacheRepositoryContext cacheRepositoryContext,
            IProgressiveCache progressiveCache,
            IIdentityService identityService,
            ISiteService siteService,
            ISiteRepository siteRepository)
        {
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _cacheRepositoryContext = cacheRepositoryContext;
            _progressiveCache = progressiveCache;
            _identityService = identityService;
            _siteService = siteService;
            _siteRepository = siteRepository;
        }

        public async Task<IEnumerable<TabItem>> GetTabsAsync(NodeIdentity parentIdentity)
        {
            // This implementation uses the NodeAliasPath, so if it's missing then we need to get it.
            if(parentIdentity.NodeAliasPathAndSiteId.HasNoValue)
            {
                var identityResult = await _identityService.HydrateNodeIdentity(parentIdentity);
                if(identityResult.IsFailure)
                {
                    return Array.Empty<TabItem>();
                }
            }

            // Should never be Maybe.None but just in case...
            if(!parentIdentity.NodeAliasPathAndSiteId.TryGetValue(out var nodeAliasPathAndSiteID))
            {
                return Array.Empty<TabItem>();
            }

            string path = nodeAliasPathAndSiteID.Item1;
            string siteName = _siteRepository.SiteNameById(nodeAliasPathAndSiteID.Item2.GetValueOrDefault(_siteService.CurrentSite.SiteID));

            var builder = _cacheDependencyBuilderFactory.Create(siteName)
                .PagePath(path, PathTypeEnum.Children);

            var nodes = (await _progressiveCache.LoadAsync(async cs =>
            {
                var tabs = await new DocumentQuery<Tab>()
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .Path(path, PathTypeEnum.Children)
                    .NestingLevel(1)
                    .OnSite(siteName)
                    .Columns(new string[] {
                    nameof(Tab.DocumentID),
                    nameof(Tab.TabName),
                    nameof(Tab.DocumentCulture)
                    })
                    .OrderBy(nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeOrder))
                    .GetEnumerableTypedResultAsync();

                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return tabs;
            }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetTabsAsync", parentIdentity.GetCacheKey()))).ToList();

            return nodes.Select(x => x.ToTabItem());
        }
    }
}
namespace CMS.DocumentEngine.Types.Generic
{
    public static class TabExtensions
    {
        public static TabItem ToTabItem(this Tab value)
        {

            return new TabItem(
                name: value.TabName,
                documentID: value.DocumentID);
        }
    }
}
