using CMS.DocumentEngine;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.Libraries.Helpers;
using Generic.Services.Interfaces;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Http;
using MVCCaching.Base.Core.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class MetaDataRepository : IMetaDataRepository
    {
        private readonly ISiteRepository _siteRepository;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        public readonly IPageRetriever _pageRetriever;
        public readonly IPageDataContextRetriever _pageDataContextRetriever;
        public readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlResolver _urlResolver;

        public MetaDataRepository(IPageRetriever pageRetriever,
            IPageDataContextRetriever pageDataContextRetriever,
            ISiteRepository siteRepository,
            ICacheDependenciesStore cacheDependenciesStore,
            IHttpContextAccessor httpContextAccessor,
            IUrlResolver urlResolver)
        {
            _pageRetriever = pageRetriever;
            _pageDataContextRetriever = pageDataContextRetriever;
            _siteRepository = siteRepository;
            _cacheDependenciesStore = cacheDependenciesStore;
            _httpContextAccessor = httpContextAccessor;
            _urlResolver = urlResolver;
        }


        public async Task<PageMetaData> GetMetaDataAsync(int documentId, string thumbnail = "")
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Page(documentId);

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentID), documentId)
                    .Columns(nameof(TreeNode.DocumentCustomData), nameof(TreeNode.DocumentPageTitle), nameof(TreeNode.DocumentPageDescription), nameof(TreeNode.DocumentPageKeyWords))
                    .TopN(1),
                cs => cs.Configure(builder, 1440, "GetMetaDataAsync", documentId)
            );
            return GetMetaDataInternal(page.FirstOrDefault(), thumbnail);
        }

        public async Task<PageMetaData> GetMetaDataAsync(Guid documentGuid, string thumbnail = "")
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Page(documentGuid);

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentGUID), documentGuid)
                    .Columns(nameof(TreeNode.DocumentCustomData), nameof(TreeNode.DocumentPageTitle), nameof(TreeNode.DocumentPageDescription), nameof(TreeNode.DocumentPageKeyWords))
                    .TopN(1),
                cs => cs.Configure(builder, 1440, "GetMetaDataAsync", documentGuid)
            );
            return GetMetaDataInternal(page.FirstOrDefault(), thumbnail);
        }

        public Task<PageMetaData> GetMetaDataAsync(string thumbnail = "")
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);

            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage))
            {
                builder.Page(currentPage.Page.DocumentID);
                return Task.FromResult(GetMetaDataInternal(currentPage.Page, thumbnail));
            } else
            {
                return Task.FromResult(GetMetaDataInternal(null, thumbnail));
            }
        }

        private PageMetaData GetMetaDataInternal(TreeNode node, string thumbnail = "", string thumbnailLarge = "")
        {
            thumbnail = !string.IsNullOrWhiteSpace(thumbnail) ? thumbnail : "";
            thumbnailLarge = !string.IsNullOrWhiteSpace(thumbnailLarge) ? thumbnailLarge : "";
            if (node == null)
            {
                return new PageMetaData()
                {
                    Title = "",
                    Keywords = "",
                    Description = "",
                    Thumbnail = !string.IsNullOrWhiteSpace(thumbnail) ? _urlResolver.GetAbsoluteUrl(thumbnail) : string.Empty,
                    ThumbnailLarge = !string.IsNullOrWhiteSpace(thumbnailLarge) ? _urlResolver.GetAbsoluteUrl(thumbnailLarge) : string.Empty
                };
            }
            else
            {
                thumbnail = !string.IsNullOrWhiteSpace(thumbnail) ? thumbnail : node.DocumentCustomData.GetValue("MetaData_ThumbnailSmall")?.ToString();
                thumbnailLarge = !string.IsNullOrWhiteSpace(thumbnailLarge) ? thumbnailLarge : node.DocumentCustomData.GetValue("MetaData_ThumbnailLarge")?.ToString();
                string keywords = node.DocumentCustomData.GetValue("MetaData_Keywords")?.ToString();
                keywords = !string.IsNullOrWhiteSpace(keywords) ? keywords : node.DocumentPageKeyWords;
                string description = node.DocumentCustomData.GetValue("MetaData_Description")?.ToString();
                description = !string.IsNullOrWhiteSpace(description) ? description : node.DocumentPageDescription;
                string title = node.DocumentCustomData.GetValue("MetaData_Title")?.ToString();
                title = !string.IsNullOrWhiteSpace(title) ? title : (!string.IsNullOrWhiteSpace(node.DocumentPageTitle) ? node.DocumentPageTitle : node.DocumentName);
                PageMetaData metaData = new PageMetaData()
                {
                    Title = title,
                    Keywords = keywords,
                    Description = description,
                    Thumbnail = !string.IsNullOrWhiteSpace(thumbnail) ? _urlResolver.GetAbsoluteUrl(thumbnail) : string.Empty,
                    ThumbnailLarge = !string.IsNullOrWhiteSpace(thumbnailLarge) ? _urlResolver.GetAbsoluteUrl(thumbnailLarge) : string.Empty
                };
                return metaData;
            }
        }

    }
}
