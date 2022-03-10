using AutoMapper;
using CMS.DocumentEngine;
using CMS.Helpers;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.Libraries.Helpers;
using Kentico.Content.Web.Mvc;
using MVCCaching;
using MVCCaching.Base.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CMS.DocumentEngine.Routing;

namespace Generic.Repositories.Implementations
{
    public class SiteMapRepository : ISiteMapRepository
    {
        private readonly IPageUrlRetriever _pageUrlRetriever;
        private readonly ISiteRepository _siteRepository;
        private readonly IMapper _mapper;
        private readonly IPageRetriever _pageRetriever;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly IRepoContext _repoContext;

        public SiteMapRepository(IPageUrlRetriever pageUrlRetriever,
            ISiteRepository siteRepository,
            IMapper mapper,
            IPageRetriever pageRetriever,
            ICacheDependenciesStore cacheDependenciesStore,
            IRepoContext repoContext)
        {
            _pageUrlRetriever = pageUrlRetriever;
            _siteRepository = siteRepository;
            _mapper = mapper;
            _pageRetriever = pageRetriever;
            _cacheDependenciesStore = cacheDependenciesStore;
            _repoContext = repoContext;
        }

        public async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetAsync()
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.PagePath("/");

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

            if (options.ClassNames?.Any() ?? false)
            {
                foreach (string ClassName in options.ClassNames)
                {
                    if (string.IsNullOrWhiteSpace(options.UrlColumnName))
                    {
                        nodes.AddRange(await GetSiteMapUrlSetForClassAsync(options.Path, ClassName, options));
                    }
                    else
                    {
                        // Since it's not the specific node, but the page found at that url that we need, we will first get the urls, then cache on getting those items.
                        nodes.AddRange(await GetSiteMapUrlSetForClassWithUrlColumnAsync(options.Path, ClassName, options));
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
            var siteMapItem = new SitemapNode()
            {
                Url = url
            };
            if (modifiedLast.HasValue)
            {
                siteMapItem.LastModificationDate = modifiedLast.Value;
            }
            return siteMapItem;
        }

        private async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetForClassAsync(string path, string className, SiteMapOptions options)
        {
            return (await GetSiteMapUrlSetForClassBase(path, className, options)).Select(x => _mapper.Map<SitemapNode>(x));
        }

        private async Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetForClassWithUrlColumnAsync(string path, string className, SiteMapOptions options)
        {
            var documents = await GetSiteMapUrlSetForClassBase(path, className, options);
            var siteMapItems = new List<SitemapNode>();

            foreach (var page in documents)
            {
                var relativeUrl = page.GetStringValue(options.UrlColumnName, _pageUrlRetriever.Retrieve(page).RelativePath);

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
                    siteMapItems.Add(ConvertToSiteMapUrl(relativeUrl, actualPage.FirstOrDefault().DocumentModifiedWhen));
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
            return (await GetSiteMapUrlSetBaseAsync(path, options)).Select(x => _mapper.Map<SitemapNode>(x));
        }

        private async Task<IEnumerable<TreeNode>> GetSiteMapUrlSetForClassBase(string path, string className, SiteMapOptions options)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.PagePath(path);

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
                if (!string.IsNullOrWhiteSpace(options.WhereCondition))
                {
                    query.Where(options.WhereCondition);
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
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.PagePath(path);

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
                if (!string.IsNullOrWhiteSpace(options.WhereCondition))
                {
                    query.Where(options.WhereCondition);
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