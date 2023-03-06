using CMS.DocumentEngine.Routing;
using Kentico.Content.Web.Mvc;

namespace Navigation.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class SiteMapRepository : ISiteMapRepository
    {
        private readonly IPageUrlRetriever _pageUrlRetriever;
        private readonly IPageRetriever _pageRetriever;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ICacheRepositoryContext _repoContext;

        public SiteMapRepository(IPageUrlRetriever pageUrlRetriever,
            IPageRetriever pageRetriever,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ICacheRepositoryContext repoContext)
        {
            _pageUrlRetriever = pageUrlRetriever;
            _pageRetriever = pageRetriever;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _repoContext = repoContext;
        }

        public async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetAsync()
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.PagePath("/", PathTypeEnum.Children);

            // You can implement your own custom data here. 
            // In this example, we will pull from any pages with the "Navigation Item" feature.
            var nodes = await _pageRetriever.RetrieveMultipleAsync(
               query => query
                    .WhereEquals(nameof(TreeNode.DocumentShowInMenu), true)
                    .OrderBy(nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeOrder))
                    .WithPageUrlPaths(),
                cacheSettings => cacheSettings
                    .Dependencies((items, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
                    .Key($"GetSiteMapUrlSetAsync|Custom")
                    .Expiration(TimeSpan.FromMinutes(60))
                    );

            return nodes.Select(x => ConvertToSiteMapUrl(_pageUrlRetriever.Retrieve(x).RelativePath, x.DocumentModifiedWhen));
        }

        public async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetAsync(SiteMapOptions options)
        {
            // Clean up
            options.Path = DataHelper.GetNotEmpty(options.Path, "/").Replace("%", "");

            var nodes = new List<SitemapNode>();

            if (options.ClassNames.Any())
            {
                foreach (string ClassName in options.ClassNames)
                {
                    if (options.UrlColumnName.TryGetValue(out var urlColumnName))
                    {
                        nodes.AddRange(await GetSiteMapUrlSetForClassAsync(options.Path, ClassName, options));
                    }
                    else
                    {
                        // Since it's not the specific node, but the page found at that url that we need, we will first get the urls, then cache on getting those items.
                        nodes.AddRange(await GetSiteMapUrlSetForClassWithUrlColumnAsync(options.Path, ClassName, options, urlColumnName));
                    }
                }
            }
            else
            {
                nodes.AddRange(await GetSiteMapUrlSetForAllClassAsync(options.Path, options));
            }

            // Clean up, remove any that are not a URL
            nodes.RemoveAll(x => !Uri.IsWellFormedUriString(x.Url, UriKind.Absolute));
            return nodes;
        }


        /// <summary>
        /// Converts the realtive Url and possible Datetime into an SitemapNode with an absolute Url
        /// </summary>
        /// <param name="RelativeURL">The Relative Url</param>
        /// <param name="ModifiedLast">The last modified date</param>
        /// <returns>The SitemapNode</returns>
        private SitemapNode ConvertToSiteMapUrl(string relativeURL, DateTime? modifiedLast)
        {
            string url = URLHelper.GetAbsoluteUrl(relativeURL, RequestContext.CurrentDomain);
            var siteMapItem = new SitemapNode(url);
            if (modifiedLast.HasValue)
            {
                siteMapItem.LastModificationDate = modifiedLast.Value;
            }
            return siteMapItem;
        }

        private async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetForClassAsync(string path, string className, SiteMapOptions options)
        {
            return (await GetSiteMapUrlSetForClassBase(path, className, options)).Select(x => TreeNodeToSitemapNode(x));
        }

        private async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetForClassWithUrlColumnAsync(string path, string className, SiteMapOptions options, string urlColumnName)
        {
            var documents = await GetSiteMapUrlSetForClassBase(path, className, options);
            var siteMapItems = new List<SitemapNode>();

            foreach (var page in documents)
            {
                var relativeUrl = page.GetStringValue(urlColumnName, _pageUrlRetriever.Retrieve(page).RelativePath);

                // Try to find page by NodeAliasPath

                var actualPage = await _pageRetriever.RetrieveAsync<TreeNode>(
                    query => query
                        .Path(relativeUrl, PathTypeEnum.Single)
                        .Columns(nameof(TreeNode.DocumentModifiedWhen))
                        ,
                    cacheSettings => cacheSettings
                        .Dependencies((items, csbuilder) => csbuilder.Pages(items))
                        .Key($"GetDocumentModified|{relativeUrl}")
                        .Expiration(TimeSpan.FromMinutes(15))
                        );

                if (actualPage.Any())
                {
                    siteMapItems.Add(ConvertToSiteMapUrl(relativeUrl, actualPage.First().DocumentModifiedWhen));
                }
                else
                {
                    siteMapItems.Add(ConvertToSiteMapUrl(relativeUrl, null));
                }
            }
            return siteMapItems;
        }

        private async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetForAllClassAsync(string path, SiteMapOptions options)
        {
            return (await GetSiteMapUrlSetBaseAsync(path, options)).Select(x => TreeNodeToSitemapNode(x));
        }

        private SitemapNode TreeNodeToSitemapNode(TreeNode node)
        {
            return new SitemapNode(node.ToPageIdentity().AbsoluteUrl)
            {
                LastModificationDate = node.DocumentModifiedWhen
            };
        }

        private async Task<IEnumerable<TreeNode>> GetSiteMapUrlSetForClassBase(string path, string className, SiteMapOptions options)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.PagePath(path, PathTypeEnum.Section);

            string culture = DataHelper.GetNotEmpty(options.CultureCode, _repoContext.CurrentCulture());

            int nodeLevel = 0;
            if (options.MaxRelativeLevel > -1)
            {
                nodeLevel = await GetNodeLevelAsync(path);
            }

            // Get the actual items
            var results = await _pageRetriever.RetrieveAsync(className, query =>
            {
                query.Path(path, PathTypeEnum.Section);
                if (options.CheckDocumentPermissions.HasValue)
                {
                    query.CheckPermissions(options.CheckDocumentPermissions.Value);
                }
                if (options.CombineWithDefaultCulture.HasValue)
                {
                    query.CombineWithDefaultCulture(options.CombineWithDefaultCulture.Value);
                }
                if (options.MaxRelativeLevel > -1)
                {
                    // Get the nesting level of the give path
                    query.NestingLevel(options.MaxRelativeLevel + nodeLevel);
                }
                if (options.WhereCondition.TryGetValue(out var whereCondition))
                {
                    query.Where(whereCondition);
                }
                query.Culture(culture)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .WithPageUrlPaths();
            }, cacheSettings =>
                cacheSettings
                    .Dependencies((items, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
                    .Key($"GetSiteMapUrlSetForClassBase|{path}{className}|{options.GetCacheKey()}")
                    .Expiration(TimeSpan.FromMinutes(1440))
                );
            return results;
        }

        private async Task<IEnumerable<TreeNode>> GetSiteMapUrlSetBaseAsync(string path, SiteMapOptions options)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.PagePath(path, PathTypeEnum.Section);

            string culture = DataHelper.GetNotEmpty(options.CultureCode, _repoContext.CurrentCulture());

            int nodeLevel = 0;
            if (options.MaxRelativeLevel > -1)
            {
                nodeLevel = await GetNodeLevelAsync(path);
            }

            // Get the actual items
            var results = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
            {
                query.Path(path, PathTypeEnum.Section);
                if (options.CheckDocumentPermissions.HasValue)
                {
                    query.CheckPermissions(options.CheckDocumentPermissions.Value);
                }
                if (options.CombineWithDefaultCulture.HasValue)
                {
                    query.CombineWithDefaultCulture(options.CombineWithDefaultCulture.Value);
                }
                if (options.MaxRelativeLevel > -1)
                {
                    // Get the nesting level of the give path
                    query.NestingLevel(options.MaxRelativeLevel + nodeLevel);
                }
                if (options.WhereCondition.TryGetValue(out var whereCondition))
                {
                    query.Where(whereCondition);
                }
                query.Culture(culture)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .WithPageUrlPaths();
            }, cacheSettings =>
                cacheSettings
                    .Dependencies((items, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
                    .Key($"GetSiteMapUrlSetBaseAsync|{path}|{options.GetCacheKey()}")
                    .Expiration(TimeSpan.FromMinutes(1440))
                );
            return results;
        }

        private async Task<int> GetNodeLevelAsync(string path)
        {
            var levelResult = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
               query
                   .Path(path, PathTypeEnum.Single)
                   .Columns(nameof(TreeNode.NodeLevel)),
               cacheSettings =>
               cacheSettings
                   .Dependencies((items, csbuilder) => csbuilder.PagePath(path, PathTypeEnum.Single))
                   .Key($"GetNodeLevelByPath|{path}")
                   .Expiration(TimeSpan.FromMinutes(60))
               );
            return levelResult.FirstOrDefault()?.NodeLevel ?? 0;
        }


    }
}