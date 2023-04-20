using CMS.Base;
using CMS.DocumentEngine.Types.SectionsSystem;
using SectionsSystem.Enums;
using SectionsSystem.Interfaces;
using SectionsSystem.Models.ImageSections;
using SectionsSystem.Models.Sections;
using SectionsSystem.Models.VideoSections;
using SectionsSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Repositories.Implementation
{
    [AutoDependencyInjection]
    public class SectionRepository : ISectionRepository
    {
        private readonly ICacheRepositoryContext _cacheRepositoryContext;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ISiteService _siteService;
        private readonly IPageRetriever _pageRetriever;
        private readonly ICoreStringToEnumParser _coreStringToEnumParser;
        private readonly IIdentityService _identityService;
        private readonly ISectionsSystemStringToEnumParser _SectionsSystemStringToEnumParser;

        public SectionRepository(ICacheRepositoryContext cacheRepositoryContext,
            IProgressiveCache progressiveCache,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ISiteService siteService,
            IPageRetriever pageRetriever,
            ICoreStringToEnumParser coreStringToEnumParser,
            IIdentityService identityService,
            ISectionsSystemStringToEnumParser SectionsSystemStringToEnumParser)
        {
            _cacheRepositoryContext = cacheRepositoryContext;
            _progressiveCache = progressiveCache;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _siteService = siteService;
            _pageRetriever = pageRetriever;
            _coreStringToEnumParser = coreStringToEnumParser;
            _identityService = identityService;
            _SectionsSystemStringToEnumParser = SectionsSystemStringToEnumParser;
        }

        #region "Modify me when adding new section"

        public static IEnumerable<string> GetSectionClassNames()
        {
            return new string[] {
                
                    SectionGeneralContent.CLASS_NAME,
                    SectionWidget.CLASS_NAME,
                    SectionFeature.CLASS_NAME
                    // Add more as you create more sections
            };
        }

        private Result<ISection> TreeNodeToSection(TreeNode node)
        {
            if (node is SectionGeneralContent sectionGeneralContent)
            {
                var position = _coreStringToEnumParser.StringToTextAlignment(sectionGeneralContent.GeneralContentTextPosition);
                var data = new GeneralContentSection(position);
                var section = new Section<GeneralContentSection>(data);
                SetSectionByTreeNode(section, node);
                return section;
            }
            else if (node is SectionWidget sectionWidget)
            {
                var data = new WidgetSection(sectionWidget.ToPageIdentity());
                var section = new Section<WidgetSection>(data);
                SetSectionByTreeNode(section, node);
                return section;
            }
            else if (node is SectionFeature sectionFeature)
            {
                var data = new FeatureSection();
                if(sectionFeature.FeatureHeading.AsNullOrWhitespaceMaybe().TryGetValue(out var featureHeading))
                {
                    data.SectionIntro = new ContentItem()
                    {
                        Header = featureHeading
                    };
                }
                var section = new Section<FeatureSection>(data);
                SetSectionByTreeNode(section, node);
                return section;
            }
            /*
            Add more as you create more sections
            */

            return Result.Failure<ISection>("Unknown node type " + node.ClassName + ", cannot convert to ISection");

        }

        #endregion

        public async Task<Maybe<ISection>> GetSectionAsync(NodeIdentity sectionIdentity)
        {
            // We use the Path, so if we don't have hte path we need to hydrate it.
            if (!sectionIdentity.NodeGuid.TryGetValue(out var nodeGuid))
            {
                if (!(await _identityService.HydrateNodeIdentity(sectionIdentity)).TryGetValue(out sectionIdentity) || !sectionIdentity.NodeGuid.TryGetValue(out nodeGuid))
                {
                    return Maybe.None;
                }
            }

            var allSectionPageTypes = GetSectionClassNames();
            var builder = _cacheDependencyBuilderFactory.Create()
                .Node(nodeGuid);

            var section = await _progressiveCache.LoadAsync(async cs =>
            {
                var sections = await new MultiDocumentQuery()
                .Types(allSectionPageTypes.ToArray())
                .WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid)
                .WithCulturePreviewModeContext(_cacheRepositoryContext)
                .WithCoupledColumns()
                .GetEnumerableTypedResultAsync();

                if (!sections.Any())
                {
                    return Maybe.None;
                }
                var item = sections.First();

                // Handle linked document
                Result<ISection> result;

                result = TreeNodeToSection(item);

                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }

                if (result.IsFailure)
                {
                    return Maybe.None;
                }
                return Maybe.From(result.Value);
            }, new CacheSettings(CacheMinuteTypes.Long.ToDouble(), "GetSection", nodeGuid, _cacheRepositoryContext.CurrentCulture()));

            return section;
        }

        public async Task<IEnumerable<ISection>> GetSectionsAsync(NodeIdentity parentIdentity)
        {
            if (!parentIdentity.NodeAliasPathAndSiteId.TryGetValue(out var nodeAliasPathTuple))
            {
                if (!(await _identityService.HydrateNodeIdentity(parentIdentity)).TryGetValue(out parentIdentity) || !parentIdentity.NodeAliasPathAndSiteId.TryGetValue(out nodeAliasPathTuple))
                {
                    return Array.Empty<ISection>();
                }
            }

            var nodeAliasPath = nodeAliasPathTuple.Item1;
            var allSectionPageTypes = GetSectionClassNames().ToList();

            var builder = _cacheDependencyBuilderFactory.Create()
                .PagePath($"{nodeAliasPath}/Sections", PathTypeEnum.Children);

            var sections = await _progressiveCache.LoadAsync(async cs =>
            {
                var sections = (await new MultiDocumentQuery()
                .Types(allSectionPageTypes.ToArray())
                .Path($"{nodeAliasPath}/Sections", PathTypeEnum.Children)
                .NestingLevel(1)
                .WithCulturePreviewModeContext(_cacheRepositoryContext)
                .CombineWithDefaultCulture()
                .WithCoupledColumns()
                .OrderByNodeOrder()
                .GetEnumerableTypedResultAsync()
                )
                .ToList();

                if (cs.Cached)
                {
                    cs.CacheDependency = _cacheDependencyBuilderFactory.Create(false)
                        .Pages(sections)
                        .AddKeys(builder.GetKeys())
                        .GetCMSCacheDependency();
                }

                // Now convert
                return sections.Select(x => TreeNodeToSection(x)).Where(x => x.IsSuccess).Select(x => x.Value);

            }, new CacheSettings(CacheMinuteTypes.Long.ToDouble(), "GetSectionsByPage", nodeAliasPath, _cacheRepositoryContext.ToCacheRepositoryContextNameIdentifier()));

            // Add document IDs and regions to the dependencies
            var validPageIdentities = sections.Select(x => x.GetSectionPageIdentity()).Where(x => x.HasValue).Select(x => x.Value);
            builder.Pages(validPageIdentities);

            return sections;
        }


        private void SetSectionByTreeNode<T>(Section<T> section, TreeNode node)
        {
            section.Page = node.ToPageIdentity();
            var nodeCustomData = node.NodeCustomData; // All Section data is in NodeCustomData through the SectionInheritedClass and XperienceCommunity.PageCustomDataControlExtender

            // Handle Section Style
            if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionStyleType), out var sectionStyleType))
            {
                switch (ValidationHelper.GetString(sectionStyleType, "Default").ToLowerInvariant())
                {
                    default:
                    case "default":
                        section.StyleType = SectionStyleType.Default;
                        break;
                    case "color":
                        section.StyleType = SectionStyleType.Color;
                        section.AdditionalCssItems.Add("bg-theme-colored");
                        break;
                    case "image":
                        section.StyleType = SectionStyleType.Image;
                        break;
                    case "parallaximage":
                        section.StyleType = SectionStyleType.ParallaxImage;
                        break;
                    case "video":
                        section.StyleType = SectionStyleType.Video;
                        break;
                }
            }

            // Theme
            if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionTheme), out var sectionTheme))
            {
                section.Theme = _SectionsSystemStringToEnumParser.StringToColorTheme(ValidationHelper.GetString(sectionTheme, "None"));
            }

            if (section.StyleType == SectionStyleType.Color && section.Theme.GetValueOrDefault(ColorTheme.None) == ColorTheme.None)
            {
                section.Theme = Maybe.None;
            }

            // Optional Additional CSS and Contrast Box
            if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionStyleAdditionalCSS), out var additionalCss))
            {
                if (ValidationHelper.GetString(additionalCss, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out var css))
                {
                    section.AdditionalCssItems.Add(css);
                }
            }




            // Light Dark Mode
            bool lightDarkSet = false;
            if (section.StyleType == SectionStyleType.Image || section.StyleType == SectionStyleType.ParallaxImage || section.StyleType == SectionStyleType.Video)
            {
                if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionLightDarkMode), out var shadeType))
                {
                    switch (_SectionsSystemStringToEnumParser.StringToShadeType(ValidationHelper.GetString(shadeType, "None")))
                    {
                        case ShadeType.Light:
                            section.AdditionalCssItems.Add("bg-light");
                            lightDarkSet = true;
                            break;
                        case ShadeType.Dark:
                            section.AdditionalCssItems.Add("bg-dark");
                            lightDarkSet = true;
                            break;
                    }
                }
            }

            // Contrast mode, only if it's Color (contrast based on color) or if Light/Dark mode is set (for image/parallax image/video)
            if (section.StyleType != SectionStyleType.Default && (section.StyleType == SectionStyleType.Color || lightDarkSet))
            {
                // Contrast Box Mode
                if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionThemeContrastBoxMode), out var contrastBoxMode))
                {
                    if (ValidationHelper.GetString(contrastBoxMode, "").AsNullOrWhitespaceMaybe().TryGetValue(out var contrastBoxModeVal))
                    {
                        switch (contrastBoxModeVal.ToLower())
                        {
                            case "none":
                                break;
                            case "overtext":
                                section.AdditionalCssItems.Add("contrast-text");
                                break;
                            case "oversection":
                                section.AdditionalCssItems.Add("contrast-section");
                                break;
                        }
                    }
                }
            }

            // Get image property
            if (section.StyleType == SectionStyleType.Image || section.StyleType == SectionStyleType.ParallaxImage)
            {
                if (section.StyleType == SectionStyleType.Image
                    && nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionImageUrl), out var sectionImageUrl)
                    && ValidationHelper.GetString(sectionImageUrl, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string sectionImageUrlVal))
                {
                    section.Image = new ImageSection(sectionImageUrlVal);
                }
                else if (section.StyleType == SectionStyleType.ParallaxImage
                    && nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionImageUrl), out var parallaxSectionImageUrl)
                    && ValidationHelper.GetString(parallaxSectionImageUrl, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string parallaxSectionImageUrlVal))
                {
                    section.Image = new ParallaxImageSection(parallaxSectionImageUrlVal);
                }
                else
                {
                    section.Image = Result.Failure<IImageSection>("No image provided, must set image url.");
                }
            }
            // Get video property
            else if (section.StyleType == SectionStyleType.Video)
            {
                if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionVideoType), out var videoType)
                           && ValidationHelper.GetString(videoType, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string videoTypeVal))
                {
                    if (videoTypeVal.Equals("Html5Video", StringComparison.OrdinalIgnoreCase))
                    {
                        Maybe<string> thumbnailUrl = Maybe.None;
                        Maybe<string> mp4Url = Maybe.None;
                        Maybe<string> webMUrl = Maybe.None;
                        Maybe<string> oggUrl = Maybe.None;
                        if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionVideoThumbnailUrl), out var thumbnail)
                            && ValidationHelper.GetString(thumbnail, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string thumbnailVal))
                        {
                            thumbnailUrl = thumbnailVal;
                        }
                        if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionVideoUrlMp4), out var mp4)
                            && ValidationHelper.GetString(mp4, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string mp4Val))
                        {
                            mp4Url = mp4Val;
                        }
                        if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionVideoUrlWebM), out var webm)
                            && ValidationHelper.GetString(webm, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string webmVal))
                        {
                            webMUrl = webmVal;
                        }
                        if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionVideoUrlOgg), out var ogg)
                            && ValidationHelper.GetString(ogg, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string oggVal))
                        {
                            oggUrl = oggVal;
                        }

                        if (mp4Url.HasValue || webMUrl.HasValue || oggUrl.HasValue)
                        {
                            if (thumbnailUrl.HasValue)
                            {
                                section.Video = new Html5VideoSection(mp4Url, webMUrl, oggUrl, thumbnailUrl.Value);
                            }
                            else
                            {
                                section.Video = new Html5VideoSection(mp4Url, webMUrl, oggUrl);
                            }
                        }
                    }
                    else if (videoTypeVal.Equals("Youtube", StringComparison.OrdinalIgnoreCase)
                        && nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionVideoYoutubeCode), out var youtube)
                        && ValidationHelper.GetString(youtube, string.Empty).AsNullOrWhitespaceMaybe().TryGetValue(out string youtubeVal))
                    {
                        section.Video = new YoutubeVideoSection(youtubeVal);
                    }
                    else
                    {
                        // Not enough data to processes
                        section.Video = Result.Failure<IVideoSection>("Not enough data to create video section. Check for missing values.");
                    }
                }
                else
                {
                    section.Video = Result.Failure<IVideoSection>("Unknown Video Type, or is not set, unable to determine Video source.");
                }
            }

            // Get show divider value
            if (nodeCustomData.TryGetValue(nameof(SectionInheritedClass.SectionShowDivider), out var dividerVal) && ValidationHelper.GetBoolean(dividerVal, false))
            {
                section.Divider = true;
            }

        }
    }
}
