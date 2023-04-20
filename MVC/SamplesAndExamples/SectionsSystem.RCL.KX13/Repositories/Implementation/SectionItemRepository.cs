using CMS.DocumentEngine.Types.SectionsSystem;
using SectionsSystem.Models.SectionItems;
using SectionsSystem.Models.Sections;
using SectionsSystem.Services;
using FeatureItem = CMS.DocumentEngine.Types.SectionsSystem.FeatureItem;

namespace SectionsSystem.Repositories.Implementation
{
    public class SectionItemRepository : ISectionItemRepository
    {

        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;
        private readonly ICoreStringToEnumParser _coreStringToEnumParser;
        private readonly ISectionsSystemStringToEnumParser _SectionsSystemStringToEnumParser;
        private readonly IVisualItemHelper _visualItemHelper;

        public SectionItemRepository(IProgressiveCache progressiveCache,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ICacheRepositoryContext cacheRepositoryContext,
            ICoreStringToEnumParser coreStringToEnumParser,
            ISectionsSystemStringToEnumParser SectionsSystemStringToEnumParser,
            IVisualItemHelper visualItemHelper)
        {
            _progressiveCache = progressiveCache;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _cacheRepositoryContext = cacheRepositoryContext;
            _coreStringToEnumParser = coreStringToEnumParser;
            _SectionsSystemStringToEnumParser = SectionsSystemStringToEnumParser;
            _visualItemHelper = visualItemHelper;
        }

        public async Task<Result<BasicSectionItem<GeneralContentSection>>> GetGeneralContentSectionItemAsync(Section<GeneralContentSection> parentSection)
        {

            var page = parentSection.GetSectionPageIdentity();
            if (page.HasNoValue)
            {
                return Result.Failure<BasicSectionItem<GeneralContentSection>>("No Page Identity found on General Content Section");
            }
            var parentSectionPage = page.Value;

            var builder = _cacheDependencyBuilderFactory.Create()
              .PagePath(parentSectionPage.Path, PathTypeEnum.Children);

            var result = await _progressiveCache.LoadAsync(async cs =>
            {
                // Get all The Basic Contnet, links, and images in one pass
                var results = await new MultiDocumentQuery()
                    .Types(new string[]
                    {
                    BasicElementVisualItem.CLASS_NAME,
                    BasicElementLink.CLASS_NAME,
                    BasicElementContent.CLASS_NAME
                    })
                    .Path(parentSectionPage.Path, PathTypeEnum.Section)
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .OrderByNodeOrder()
                    // Only need coupled columns in this case
                    .OnlyCoupledColumns()
                    .GetEnumerableTypedResultAsync();

                var visualItems = new List<IVisualItem>();
                var contentItems = new List<ContentItem>();
                var linkItems = new List<ILinkItem>();

                foreach (var node in results)
                {
                    if (node is BasicElementVisualItem visualItem)
                    {
                        // General Link first
                        Maybe<GeneralLink> generalLink = Maybe.None;
                        if (visualItem.VisualItemHasLink && visualItem.VisualItemLinkUrl.AsNullOrWhitespaceMaybe().TryGetValue(out var linkUrl))
                        {
                            var target = _coreStringToEnumParser.StringToLinkTargetType(visualItem.VisualItemLinkTarget);
                            generalLink = new GeneralLink(linkUrl, target);
                        }
                        var item = _visualItemHelper.GenerateVisualItem(_coreStringToEnumParser.StringToVisualItemType(visualItem.VisualItemType), visualItem.VisualItemIconSource, visualItem.VisualItemFontAwesomeIconCode, visualItem.VisualItemCanvasIconCode, visualItem.VisualItemCustomIconCode, visualItem.VisualItemIconAlt, visualItem.VisualItemImageUrl, visualItem.VisualItemImageAlt, generalLink);
                        visualItems.Add(item);
                    }
                    if (node is BasicElementContent contentItem)
                    {
                        contentItems.Add(new ContentItem()
                        {
                            Header = contentItem.ContentHeading.AsNullOrWhitespaceMaybe(),
                            SubHeader = contentItem.ContentSubHeading.AsNullOrWhitespaceMaybe(),
                            HtmlContent = contentItem.ContentHtml.AsNullOrWhitespaceMaybe()
                        });
                    }
                    if (node is BasicElementLink linkItem)
                    {
                        var target = _coreStringToEnumParser.StringToLinkTargetType(linkItem.LinkTarget);
                        var linkUrl = linkItem.LinkUrl;
                        ILinkItem link = linkItem.LinkType.ToLower() switch
                        {
                            "text" => new TextLink(linkUrl, target, linkItem.LinkText),
                            "button" => new ButtonLink(linkUrl, target, linkItem.LinkText),
                            "general" => new GeneralLink(linkUrl, target),
                            _ => new GeneralLink(linkUrl, target)
                        };

                        linkItems.Add(link);
                    }
                }

                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }

                if (!visualItems.Any() && !contentItems.Any() && !linkItems.Any())
                {
                    return Result.Failure<BasicSectionItem<GeneralContentSection>>("No child elements found.");
                }

                return new BasicSectionItem<GeneralContentSection>(parentSection.SectionModel)
                {
                    ContentItems = contentItems,
                    LinkItems = linkItems,
                    VisualItems = visualItems
                };

            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(),
                "GetGeneralContentSectionItem",
        parentSectionPage.ToCacheNameIdentifier(),
        _cacheRepositoryContext.ToCacheRepositoryContextNameIdentifier()));

            return result;
        }

        public async Task<IEnumerable<BasicSectionItem>> GetFeatureSectionItemsAsync(PageIdentity parentPage)
        {
            var builder = _cacheDependencyBuilderFactory.Create()
            .PagePath(parentPage.Path, PathTypeEnum.Children);


            var featureItems = await _progressiveCache.LoadAsync(async cs =>
            {
                // Get all The Basic Contnet, links, and images in one pass
                var results = await new DocumentQuery<FeatureItem>()
                    .Path(parentPage.Path, PathTypeEnum.Children)
                    .WithCulturePreviewModeContext(_cacheRepositoryContext)
                    .OrderByNodeOrder()
                    // Only need coupled columns in this case
                    .Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(FeatureItem.FeatureItemTitle),
                        nameof(FeatureItem.FeatureItemDescription),
                        nameof(FeatureItem.FeatureItemIconSource),
                        nameof(FeatureItem.FeatureItemIconAlt),
                        nameof(FeatureItem.FeatureItemFontAwesomeIconCode),
                        nameof(FeatureItem.FeatureItemCanvasIconCode),
                        nameof(FeatureItem.FeatureItemCustomIconCode),
                        nameof(FeatureItem.FeatureItemLinkUrl),
                        nameof(FeatureItem.FeatureItemLinkTarget)
                    })
                    .GetEnumerableTypedResultAsync();


                // combine and add to cache dependencies
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }

                return results;
            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(),
                "GetFeatureSectionItems",
                parentPage.ToCacheNameIdentifier(),
                _cacheRepositoryContext.ToCacheRepositoryContextNameIdentifier()));

            return featureItems.Select(x =>
            {
                var basicSectionItem = new BasicSectionItem()
                {
                    ContentItems = new List<ContentItem>() {
                    new ContentItem()
                    {
                        Header = x.FeatureItemTitle,
                        HtmlContent = x.FeatureItemDescription.AsNullOrWhitespaceMaybe()
                    }
                }
                };

                Maybe<GeneralLink> generalLink = Maybe.None;
                if (x.FeatureItemLinkUrl.AsNullOrWhitespaceMaybe().TryGetValue(out var linkUrl))
                {
                    generalLink = new GeneralLink(linkUrl, _coreStringToEnumParser.StringToLinkTargetType(x.FeatureItemLinkTarget));
                    basicSectionItem.LinkItems.Add(generalLink.Value);
                }

                basicSectionItem.VisualItems.Add(_visualItemHelper.GenerateVisualItem(VisualItemType.Icon, x.FeatureItemIconSource, x.FeatureItemFontAwesomeIconCode, x.FeatureItemCanvasIconCode, x.FeatureItemCustomIconCode, x.FeatureItemIconAlt, String.Empty, String.Empty, generalLink));

                return basicSectionItem;
            });

        }
    }
}
