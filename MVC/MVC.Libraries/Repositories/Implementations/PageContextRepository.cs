using AutoMapper;
using CMS.DocumentEngine;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.Libraries.Helpers;
using Kentico.Content.Web.Mvc;
using MVCCaching.Base.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CMS.Base.Internal;
using Kentico.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Generic.Libraries.Extensions;
using CMS.DocumentEngine.Routing;

namespace Generic.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class PageContextRepository : IPageContextRepository
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;
        private readonly IPageRetriever _pageRetriever;
        private readonly IMapper _mapper;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteRepository _siteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PageContextRepository(IPageDataContextRetriever pageDataContextRetriever,
            IPageRetriever pageRetriever,
            IMapper mapper,
            ICacheDependenciesStore cacheDependenciesStore,
            ISiteRepository siteRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _pageDataContextRetriever = pageDataContextRetriever;
            _pageRetriever = pageRetriever;
            _mapper = mapper;
            _cacheDependenciesStore = cacheDependenciesStore;
            _siteRepository = siteRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<PageIdentity> GetCurrentPageAsync()
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage))
            {
                builder.Node(currentPage.Page.NodeID);
                return Task.FromResult((PageIdentity)currentPage.Page.ToPageIdentity());
            }
            else
            {
                return Task.FromResult<PageIdentity>(null);
            }
        }

        public async Task<PageIdentity> GetPageAsync(string path)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.PagePath(path, PathTypeEnum.Single);

            // Check current page
            var currentPage = await GetCurrentPageAsync();
            if ((currentPage?.Path ?? "").Equals(path, StringComparison.InvariantCultureIgnoreCase))
            {
                return currentPage;
            }

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .Path(path, PathTypeEnum.Single)
                    .Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel)
                        })
                    .WithPageUrlPaths(),
                cs => cs.Configure(builder, 15, "GetPageAsync", path)
                );
            return (page.Any() ? page.FirstOrDefault().ToPageIdentity() : null);
        }

        public async Task<PageIdentity> GetPageAsync(int documentID)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Page(documentID);

            // Check current page
            var currentPage = await GetCurrentPageAsync();
            if ((currentPage?.DocumentID ?? 0) == documentID)
            {
                return currentPage;
            }

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentID), documentID)
                    .Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel)
                        })
                    .WithPageUrlPaths(),
                cs => cs.Configure(builder, 15, "GetPageAsync", documentID)
                );
            return (page.Any() ? page.FirstOrDefault().ToPageIdentity() : null);
        }

        public async Task<PageIdentity> GetPageAsync(Guid documentGUID)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Page(documentGUID);

            // Check current page
            var currentPage = await GetCurrentPageAsync();
            if ((currentPage?.DocumentGUID ?? Guid.Empty).Equals(documentGUID))
            {
                return currentPage;
            }

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentGUID), documentGUID)
                    .Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel)
                        })
                    .WithPageUrlPaths(),
                cs => cs.Configure(builder, 15, "GetPageAsync", documentGUID)
                );
            return (page.Any() ? page.FirstOrDefault().ToPageIdentity() : null);
        }

        public async Task<PageIdentity> GetPageByNodeAsync(int nodeID)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Node(nodeID);

            // Check current page
            var currentPage = await GetCurrentPageAsync();
            if ((currentPage?.NodeID ?? 0).Equals(nodeID))
            {
                return currentPage;
            }

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.NodeID), nodeID)
                    .Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel)
                        })
                    .WithPageUrlPaths(),
                cs => cs.Configure(builder, 15, "GetPageByNodeAsync", nodeID)
                );
            return (page.Any() ? page.FirstOrDefault().ToPageIdentity() : null);
        }

        public async Task<PageIdentity> GetPageByNodeAsync(Guid nodeGUID)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Node(nodeGUID);

            // Check current page
            var currentPage = await GetCurrentPageAsync();
            if ((currentPage?.NodeGUID ?? Guid.Empty).Equals(nodeGUID))
            {
                return currentPage;
            }

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.NodeGUID), nodeGUID)
                    .Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel)
                        })
                    .WithPageUrlPaths(),
                cs => cs.Configure(builder, 15, "GetPageByNodeAsync", nodeGUID)
                );
            return (page.Any() ? page.FirstOrDefault().ToPageIdentity() : null);
        }

        public Task<bool> IsEditModeAsync()
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.Kentico().PageBuilder().EditMode);
        }
    }
}
