using CMS.DataEngine;
using CMS.Localization;
using Kentico.Content.Web.Mvc;
using Microsoft.Extensions.Localization;

namespace Navigation.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class BreadcrumbRepository : IBreadcrumbRepository
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IUrlResolver _urlResolver;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly IPageRetriever _pageRetriever;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheRepositoryContext _repoContext;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public BreadcrumbRepository(ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ISiteRepository siteRepository,
            IUrlResolver urlResolver,
            IPageRetriever pageRetriever,
            IProgressiveCache progressiveCache,
            ICacheRepositoryContext repoContext,
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            _siteRepository = siteRepository;
            _urlResolver = urlResolver;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _pageRetriever = pageRetriever;
            _progressiveCache = progressiveCache;
            _repoContext = repoContext;
            _stringLocalizer = stringLocalizer;
        }

        public Task<BreadcrumbJsonLD> BreadcrumbsToJsonLDAsync(IEnumerable<Breadcrumb> breadcrumbs, bool excludeFirst = true)
        {
            var itemListElement = new List<ItemListElementJsonLD>();
            int position = 0;
            foreach (Breadcrumb breadcrumb in (excludeFirst ? breadcrumbs.Skip(1) : breadcrumbs))
            {
                position++;
                itemListElement.Add(new ItemListElementJsonLD(
                    position: position,
                    name: breadcrumb.LinkText,
                    item: _urlResolver.GetAbsoluteUrl(breadcrumb.LinkUrl)
                    )
               );
            }

            return Task.FromResult(new BreadcrumbJsonLD(itemListElement));
        }

        public async Task<List<Breadcrumb>> GetBreadcrumbsAsync(int nodeID, bool includeDefaultBreadcrumb = true)
        {
            var builder = _cacheDependencyBuilderFactory.Create();

            var breadcrumbsDictionary = await GetNodeToBreadcrumbAndParent();

            bool isCurrentPage = true;
            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>();
            int nextNodeID = nodeID;
            while (breadcrumbsDictionary.ContainsKey(nextNodeID))
            {
                // Add dependency
                builder.Node(nextNodeID);

                // Create or get breadcrumb
                var breadcrumbTuple = breadcrumbsDictionary[nextNodeID];
                var breadcrumb = !isCurrentPage ? breadcrumbTuple.Item1 : new Breadcrumb(linkText: breadcrumbTuple.Item1.LinkText, linkUrl: breadcrumbTuple.Item1.LinkUrl, true);
                isCurrentPage = false;
                if (!string.IsNullOrWhiteSpace(breadcrumb.LinkText))
                {
                    breadcrumbs.Add(breadcrumb);
                }
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
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Object(ResourceStringInfo.OBJECT_TYPE, "generic.default.breadcrumbtext")
                    .Object(ResourceStringInfo.OBJECT_TYPE, "generic.default.breadcrumburl");

            return Task.FromResult(_progressiveCache.Load(cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(builder.GetKeys().ToArray());
                }
                return new Breadcrumb(linkText: _stringLocalizer.GetString("generic.default.breadcrumbtext"),
                    linkUrl: _stringLocalizer.GetString("generic.default.breadcrumburl")
                    );
            }, new CacheSettings(1440, "GetDefaultBreadcrumb", _repoContext.CurrentCulture())));
        }

        private async Task<Dictionary<int, Tuple<Breadcrumb, int>>> GetNodeToBreadcrumbAndParent()
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Object(SettingsKeyInfo.OBJECT_TYPE, "BreadcrumbPageTypes");

            string[] validClassNames = SettingsKeyInfoProvider.GetValue(new SettingsKeyName("BreadcrumbPageTypes", _siteRepository.CurrentSiteID())).ToLower().Split(";,|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            
            // Cache dependency should not extend to the CacheDependenciesStore as only the matching breadcrumbs should apply.
            var results = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .IncludePageIdentityColumns()
                    .Columns(nameof(TreeNode.NodeParentID), nameof(TreeNode.DocumentName))
                    .If(validClassNames.Any(), query => query.Where($"NodeClassID in (select ClassID from CMS_Class where ClassName in ('{string.Join("','", validClassNames.Select(x => SqlHelper.EscapeQuotes(x)))}'))")),
                cacheSettings => cacheSettings
                    .Dependencies((items, csbuilder) => csbuilder.PagePath("/", PathTypeEnum.Section))
                    .Key($"GetNodeToBreadcrumbAndParent") // Since Page Retriever, already has context of culture and site and published
                    .Expiration(TimeSpan.FromMinutes(60))
                );

            return results.GroupBy(x => x.NodeID)
                .ToDictionary(key => key.Key, values => values.Select(x =>
                {
                    string linkText = x.DocumentName;
                    return new Tuple<Breadcrumb, int>(new Breadcrumb(linkText: linkText, linkUrl: x.ToPageIdentity().RelativeUrl), x.NodeParentID);
                }).First());
        }
    }
}