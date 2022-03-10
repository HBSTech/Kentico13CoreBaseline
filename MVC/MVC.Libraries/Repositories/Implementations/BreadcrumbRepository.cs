using AutoMapper;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using Generic.Libraries.Helpers;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Kentico.Content.Web.Mvc;
using MVCCaching;
using MVCCaching.Base.Core.Interfaces;
using MVCCaching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    public class BreadcrumbRepository : IBreadcrumbRepository
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IUrlResolver _urlResolver;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly IPageRetriever _pageRetriever;
        private readonly IMapper _mapper;
        private readonly IProgressiveCache _progressiveCache;
        private readonly IRepoContext _repoContext;

        public BreadcrumbRepository(ICacheDependenciesStore cacheDependenciesStore,
            ISiteRepository siteRepository,
            IMapper mapper,
            IUrlResolver urlResolver,
            IPageRetriever pageRetriever,
            IProgressiveCache progressiveCache,
            IRepoContext  repoContext)
        {
            _siteRepository = siteRepository;
            _urlResolver = urlResolver;
            _cacheDependenciesStore = cacheDependenciesStore;
            _pageRetriever = pageRetriever;
            _mapper = mapper;
            _progressiveCache = progressiveCache;
            _repoContext = repoContext;
        }

        public Task<BreadcrumbJsonLD> BreadcrumbsToJsonLDAsync(IEnumerable<Breadcrumb> breadcrumbs, bool excludeFirst = true)
        {
            var itemListElement = new List<ItemListElementJsonLD>();
            int position = 0;
            foreach (Breadcrumb breadcrumb in (excludeFirst ? breadcrumbs.Skip(1) : breadcrumbs))
            {
                position++;
                itemListElement.Add(new ItemListElementJsonLD()
                {
                    position = position,
                    Name = breadcrumb.LinkText,
                    Item = _urlResolver.GetAbsoluteUrl(breadcrumb.LinkUrl)
                });
            }
            
            return Task.FromResult(new BreadcrumbJsonLD()
            {
                itemListElement = itemListElement
            });
        }

        public async Task<List<Breadcrumb>> GetBreadcrumbsAsync(int nodeID, bool includeDefaultBreadcrumb = true)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);

            var breadcrumbsDictionary = await GetNodeToBreadcrumbAndParent();

            bool isCurrentPage = true;
            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>();
            int nextNodeID = nodeID;
            while(breadcrumbsDictionary.ContainsKey(nextNodeID))
            {
                // Add dependency
                builder.Node(nextNodeID);

                // Create or get breadcrumb
                var breadcrumbTuple = breadcrumbsDictionary[nextNodeID];
                Breadcrumb breadcrumb = !isCurrentPage ? breadcrumbTuple.Item1 : new Breadcrumb()
                {
                    IsCurrentPage = true,
                    LinkText = breadcrumbTuple.Item1.LinkText,
                    LinkUrl = breadcrumbTuple.Item1.LinkUrl
                };
                isCurrentPage = false;

                breadcrumbs.Add(breadcrumb);
                nextNodeID = breadcrumbTuple.Item2;
            }

            // Add given Top Level Breadcrumb if provided
            if (includeDefaultBreadcrumb)
            {
                breadcrumbs.Add(await GetDefaultBreadcrumbAsync());
            }

            // Reverse breadcrumb order
            breadcrumbs.Reverse();
            return breadcrumbs;
        }

        public Task<Breadcrumb> GetDefaultBreadcrumbAsync()
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(ResourceStringInfo.OBJECT_TYPE, "generic.default.breadcrumbtext")
                    .Object(ResourceStringInfo.OBJECT_TYPE, "generic.default.breadcrumburl");

            return Task.FromResult(_progressiveCache.Load(cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return new Breadcrumb()
                {
                    LinkText = ResHelper.LocalizeExpression("generic.default.breadcrumbtext"),
                    LinkUrl = ResHelper.LocalizeExpression("generic.default.breadcrumburl"),
                };
            }, new CacheSettings(1440, "GetDefaultBreadcrumb", _repoContext.CurrentCulture())));
        }

        private async Task<Dictionary<int, Tuple<Breadcrumb, int>>> GetNodeToBreadcrumbAndParent()
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(SettingsKeyInfo.OBJECT_TYPE, "BreadcrumbPageTypes");

            string[] validClassNames = SettingsKeyInfoProvider.GetValue(new SettingsKeyName("BreadcrumbPageTypes", new SiteInfoIdentifier(SiteContext.CurrentSiteID))).ToLower().Split(";,|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Cache dependency should not extend to the CacheDependenciesStore as only the matching breadcrumbs should apply.
            var results = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .Where($"NodeClassID not in (select ClassID from CMS_Class where ClassName in ('{string.Join("','", validClassNames.Select(x => SqlHelper.EscapeQuotes(x)))}'))"),
                cacheSettings => cacheSettings
                    .Dependencies((items, csbuilder) => csbuilder.PagePath("/", PathTypeEnum.Section))
                    .Key($"GetNodeToBreadcrumbAndParent")
                    .Expiration(TimeSpan.FromMinutes(60))
                );
            return results.GroupBy(x => x.NodeID)
                .ToDictionary(key => key.Key, values => values.Select(x => new Tuple<Breadcrumb, int>(_mapper.Map<Breadcrumb>(x), x.NodeParentID)).FirstOrDefault());
        }
    }
}