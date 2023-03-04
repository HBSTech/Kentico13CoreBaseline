using CMS.Base;
using Microsoft.AspNetCore.Http;

namespace Core.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class MetaDataRepository : IMetaDataRepository
    {
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        public readonly IPageRetriever _pageRetriever;
        public readonly IPageDataContextRetriever _pageDataContextRetriever;
        public readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlResolver _urlResolver;
        private readonly IMediaRepository _mediaRepository;
        private readonly ISiteService _siteService;

        public MetaDataRepository(IPageRetriever pageRetriever,
            IPageDataContextRetriever pageDataContextRetriever,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            IHttpContextAccessor httpContextAccessor,
            IUrlResolver urlResolver,
            IMediaRepository mediaRepository,
            ISiteService siteService)
        {
            _pageRetriever = pageRetriever;
            _pageDataContextRetriever = pageDataContextRetriever;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _httpContextAccessor = httpContextAccessor;
            _urlResolver = urlResolver;
            _mediaRepository = mediaRepository;
            _siteService = siteService;
        }


        public async Task<Result<PageMetaData>> GetMetaDataAsync(int documentId, string? thumbnail = null)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Page(documentId);

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentID), documentId)
                    .Columns(nameof(TreeNode.DocumentCustomData), nameof(TreeNode.DocumentPageTitle), nameof(TreeNode.DocumentPageDescription), nameof(TreeNode.DocumentPageKeyWords))
                    .TopN(1),
                cacheSettings => cacheSettings
                    .Dependencies((result, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
                    .Key($"GetMetaDataAsync|{documentId}")
                    .Expiration(TimeSpan.FromMinutes(1440))
            );
            if (page.Any())
            {
                return await GetMetaDataInternalAsync(page.First(), thumbnail);
            }
            else
            {
                return Result.Failure<PageMetaData>("No page found by that documentID");
            }
        }

        public async Task<Result<PageMetaData>> GetMetaDataAsync(Guid documentGuid, string? thumbnail = null)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Page(documentGuid);

            var page = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentGUID), documentGuid)
                    .Columns(nameof(TreeNode.DocumentCustomData), nameof(TreeNode.DocumentPageTitle), nameof(TreeNode.DocumentPageDescription), nameof(TreeNode.DocumentPageKeyWords))
                    .TopN(1),
                cacheSettings => cacheSettings
                    .Dependencies((result, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
                    .Key($"GetMetaDataAsync|{documentGuid}")
                    .Expiration(TimeSpan.FromDays(1))
            );
            if (page.Any())
            {
                return await GetMetaDataInternalAsync(page.First(), thumbnail);
            }
            else
            {
                return Result.Failure<PageMetaData>("No page found by that documentGuid");
            }
        }

        public async Task<Result<PageMetaData>> GetMetaDataAsync(string? thumbnail = null)
        {
            var builder = _cacheDependencyBuilderFactory.Create();

            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage))
            {
                builder.Page(currentPage.Page.DocumentID);
                return await GetMetaDataInternalAsync(currentPage.Page, thumbnail);
            }
            else
            {
                return Result.Failure<PageMetaData>("No page in the current context");
            }
        }

        private Task<PageMetaData> GetMetaDataInternalAsync(TreeNode node, string? thumbnail = null, string? thumbnailLarge = null)
        {
            string keywords = string.Empty;
            string description = string.Empty;
            string title = string.Empty;

            // Try to get these values first
            /*if (node is SomePageType menuPage)
            {
                if (menuPage.MenuItemTeaserImage.WithDefaultAsNone().TryGetValue(out var teaserGuid))
                {
                    var thumbResult = await _mediaRepository.GetAttachmentItemAsync(teaserGuid);
                    if (thumbResult.TryGetValue(out var thumb))
                    {
                        thumbnail = thumb.AttachmentUrl;
                    }
                }
                if (menuPage.Keywords.AsNullOrWhitespaceMaybe().TryGetValue(out var keywordsVal))
                {
                    keywords = keywordsVal;
                }
                if (menuPage.MenuItemSeoTitleOverride.AsNullOrWhitespaceMaybe().TryGetValue(out var seoTitleOverride))
                {
                    title = seoTitleOverride;
                }
            }
            */


            // Document custom data overrides, then site default
            thumbnail = thumbnail.AsNullOrWhitespaceMaybe().GetValueOrDefault(node.DocumentCustomData.GetValue("MetaData_ThumbnailSmall").GetValueOrDefault(string.Empty).ToString());
            thumbnailLarge = thumbnailLarge.AsNullOrWhitespaceMaybe().GetValueOrDefault(node.DocumentCustomData.GetValue("MetaData_ThumbnailLarge").GetValueOrDefault(string.Empty).ToString());
            keywords = keywords.AsNullOrWhitespaceMaybe().GetValueOrDefault(node.DocumentCustomData.GetValue("MetaData_Keywords").GetValueOrDefault(node.DocumentPageKeyWords).ToString()).AsNullOrWhitespaceMaybe().GetValueOrDefault();
            description = description.AsNullOrWhitespaceMaybe().GetValueOrDefault(node.DocumentCustomData.GetValue("MetaData_Description").GetValueOrDefault(node.DocumentPageDescription).ToString());
            title = title.AsNullOrWhitespaceMaybe().GetValueOrDefault(node.DocumentCustomData.GetValue("MetaData_Title").GetValueOrDefault(node.DocumentPageTitle).ToString().AsNullOrWhitespaceMaybe().GetValueOrDefault(node.DocumentName));

            PageMetaData metaData = new PageMetaData()
            {
                Title = title,
                Keywords = keywords.AsNullOrWhitespaceMaybe().GetValueOrDefault(string.Empty),
                Description = description.AsNullOrWhitespaceMaybe().GetValueOrDefault(string.Empty),
                Thumbnail = thumbnail.AsNullOrWhitespaceMaybe().TryGetValue(out var thumbUrl) ? _urlResolver.GetAbsoluteUrl(thumbUrl) : string.Empty,
                ThumbnailLarge = thumbnailLarge.AsNullOrWhitespaceMaybe().TryGetValue(out var thumbLargeUrl) ? _urlResolver.GetAbsoluteUrl(thumbLargeUrl) : string.Empty,
            };
            return Task.FromResult(metaData);
        }

    }
}
