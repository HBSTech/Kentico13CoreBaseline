using Microsoft.AspNetCore.Http;
using CMS.Base.Internal;
using Kentico.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using CMS.DocumentEngine.Routing;
using Core.Extensions;
using MVCCaching;

namespace Core.Repositories.Implementation
{
    [AutoDependencyInjection]
    public class PageContextRepository : IPageContextRepository
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;
        private readonly IPageRetriever _pageRetriever;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;

        public PageContextRepository(IPageDataContextRetriever pageDataContextRetriever,
            IPageRetriever pageRetriever,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            IHttpContextAccessor httpContextAccessor,
            IProgressiveCache progressiveCache,
            ICacheRepositoryContext cacheRepositoryContext
            )
        {
            _pageDataContextRetriever = pageDataContextRetriever;
            _pageRetriever = pageRetriever;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _httpContextAccessor = httpContextAccessor;
            _progressiveCache = progressiveCache;
            _cacheRepositoryContext = cacheRepositoryContext;
        }

        public Task<Result<PageIdentity>> GetCurrentPageAsync()
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage))
            {
                builder.Node(currentPage.Page.NodeID);
                return Task.FromResult((Result<PageIdentity>)currentPage.Page.ToPageIdentity());
            }
            else
            {
                return Task.FromResult(Result.Failure<PageIdentity>("Could not resolve page"));
            }
        }

        public async Task<Result<PageIdentity>> GetPageAsync(NodeIdentity identity)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            var currentPage = await GetCurrentPageAsync();

            if (identity.NodeId.TryGetValue(out var nodeId))
            {
                if(currentPage.TryGetValue(out var currentPageVal) && currentPageVal.NodeID == nodeId){
                    return currentPage;
                }
                builder.Node(nodeId);
                return await _progressiveCache.LoadAsync(async cs =>
                {
                    var result = await DocumentHelper.GetDocuments()
                    .WhereEquals(nameof(TreeNode.NodeID), nodeId)
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .IncludePageIdentityColumns()
                    .TopN(1)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .GetEnumerableTypedResultAsync();

                    return result.Any() ? result.First().ToPageIdentity() : Result.Failure<PageIdentity>($"Could not find a page with NodeID {nodeId}");

                }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetPageAsyncByNodeID", nodeId));
            }
            if (identity.NodeGuid.TryGetValue(out var nodeGuid))
            {
                if (currentPage.TryGetValue(out var currentPageVal) && currentPageVal.NodeGUID == nodeGuid)
                {
                    return currentPage;
                }
                builder.Node(nodeGuid);
                return await _progressiveCache.LoadAsync(async cs =>
                {
                    var result = await DocumentHelper.GetDocuments()
                    .WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid)
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .IncludePageIdentityColumns()
                    .TopN(1)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .GetEnumerableTypedResultAsync();

                    return result.Any() ? result.First().ToPageIdentity() : Result.Failure<PageIdentity>($"Could not find a page with NodeGuid {nodeGuid}");

                }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetPageAsyncByNodeGuid", nodeGuid));
            }
            if (identity.NodeAliasPathAndSiteId.TryGetValue(out var nodeAliasPathAndSiteId))
            {
                if (currentPage.TryGetValue(out var currentPageVal) && currentPageVal.Path.Equals(nodeAliasPathAndSiteId.Item1, StringComparison.OrdinalIgnoreCase))
                {
                    return currentPage;
                }
                builder.PagePath(nodeAliasPathAndSiteId.Item1);
                return await _progressiveCache.LoadAsync(async cs =>
                {
                    var result = await DocumentHelper.GetDocuments()
                    .WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid)
                    .If(nodeAliasPathAndSiteId.Item2.TryGetValue(out var nodeSiteID), query => query.OnSite(nodeSiteID, false))
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .IncludePageIdentityColumns()
                    .TopN(1)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .GetEnumerableTypedResultAsync();

                    return result.Any() ? result.First().ToPageIdentity() : Result.Failure<PageIdentity>($"Could not find a page with NodeGuid {nodeGuid}");

                }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetPageAsyncByNodePath", nodeAliasPathAndSiteId.Item1, nodeAliasPathAndSiteId.Item2.GetValueOrDefault(0)));
            }
            return Result.Failure<PageIdentity>("No identifier was passed");
        }

        public async Task<Result<PageIdentity>> GetPageAsync(DocumentIdentity identity)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            var currentPage = await GetCurrentPageAsync();

            if (identity.DocumentId.TryGetValue(out var documentId))
            {
                if (currentPage.TryGetValue(out var currentPageVal) && currentPageVal.DocumentID == documentId)
                {
                    return currentPage;
                }
                builder.Page(documentId);
                return await _progressiveCache.LoadAsync(async cs =>
                {
                    if (cs.Cached)
                    {
                        cs.CacheDependency = builder.GetCMSCacheDependency();
                    }

                    var result = await DocumentHelper.GetDocuments()
                    .WhereEquals(nameof(TreeNode.DocumentID), documentId)
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .IncludePageIdentityColumns()
                    .TopN(1)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .GetEnumerableTypedResultAsync();

                    return result.Any() ? result.First().ToPageIdentity() : Result.Failure<PageIdentity>($"Could not find a page with DocumentID {documentId}");

                }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetPageAsyncByDocumentID", documentId));
            }
            if (identity.DocumentGuid.TryGetValue(out var documentGuid))
            {
                if (currentPage.TryGetValue(out var currentPageVal) && currentPageVal.DocumentGUID == documentGuid)
                {
                    return currentPage;
                }
                builder.Page(documentGuid);
                return await _progressiveCache.LoadAsync(async cs =>
                {
                    if (cs.Cached)
                    {
                        cs.CacheDependency = builder.GetCMSCacheDependency();
                    }

                    var result = await DocumentHelper.GetDocuments()
                    .WhereEquals(nameof(TreeNode.DocumentGUID), documentGuid)
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .IncludePageIdentityColumns()
                    .TopN(1)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .GetEnumerableTypedResultAsync();

                    return result.Any() ? result.First().ToPageIdentity() : Result.Failure<PageIdentity>($"Could not find a page with DocumentGuid {documentGuid}");

                }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetPageAsyncByDocumentGuid", documentGuid));
            }
            if (identity.NodeAliasPathAndMaybeCultureAndSiteId.TryGetValue(out var nodeAliasPathAndMaybeCultureAndSiteId))
            {
                if (currentPage.TryGetValue(out var currentPageVal) && 
                    currentPageVal.Path.Equals(nodeAliasPathAndMaybeCultureAndSiteId.Item1, StringComparison.OrdinalIgnoreCase)
                    && (nodeAliasPathAndMaybeCultureAndSiteId.Item3.TryGetValue(out var siteID) ? currentPageVal.NodeSiteID == siteID : true)
                    && (nodeAliasPathAndMaybeCultureAndSiteId.Item2.TryGetValue(out var culture) ? currentPageVal.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase) : true)
                    )
                {
                    return currentPage;
                }
                builder.PagePath(nodeAliasPathAndMaybeCultureAndSiteId.Item1);
                var test =  await _progressiveCache.LoadAsync(async cs =>
                {
                    if(cs.Cached)
                    {
                        cs.CacheDependency = builder.GetCMSCacheDependency();
                    }

                    var result = await DocumentHelper.GetDocuments()
                    .Path(nodeAliasPathAndMaybeCultureAndSiteId.Item1, PathTypeEnum.Explicit)
                    .If(nodeAliasPathAndMaybeCultureAndSiteId.Item2.TryGetValue(out var culture), query => query.Culture(culture))
                    .If(nodeAliasPathAndMaybeCultureAndSiteId.Item3.TryGetValue(out var nodeSiteID), query => query.OnSite(nodeSiteID, false))
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .IncludePageIdentityColumns()
                    .TopN(1)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .GetEnumerableTypedResultAsync();

                    return result.Any() ? result.First().ToPageIdentity() : Result.Failure<PageIdentity>($"Could not find a page with NodeAliaPath {nodeAliasPathAndMaybeCultureAndSiteId.Item1}");

                }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetPageAsyncByDocumentNodeAliasPathAndCultureSite", nodeAliasPathAndMaybeCultureAndSiteId.Item1, nodeAliasPathAndMaybeCultureAndSiteId.Item2.GetValueOrDefault("SiteCulture"), nodeAliasPathAndMaybeCultureAndSiteId.Item3.GetValueOrDefault(Maybe.None)));
            }
            return Result.Failure<PageIdentity>("No identifier was passed");
        }


        public Task<bool> IsEditModeAsync()
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.Kentico().PageBuilder().EditMode);
        }
    }
}
