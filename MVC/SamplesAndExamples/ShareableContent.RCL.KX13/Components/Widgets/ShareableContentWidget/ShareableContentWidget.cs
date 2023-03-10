using CMS.DocumentEngine;
using Core.Enums;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Content.Web.Mvc;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using MVCCaching.Base.Core.Interfaces;
using ShareableContent.Components.Widgets.ShareableContentWidget;
using ShareableContentPageType = CMS.DocumentEngine.Types.Generic.ShareableContent;

[assembly: RegisterWidget(ShareableContentWidgetViewComponent.IDENTITY,
    typeof(ShareableContentWidgetViewComponent),
                         "Shareable Content",
    typeof(ShareableContentWidgetProperties),
                         Description = "Displays the widget content of a Shareable Content Page", IconClass = "icon-recaptcha", AllowCache = true)]
namespace ShareableContent.Components.Widgets.ShareableContentWidget
{
    [CacheVaryBy(VaryByCulture = true)]
    public class ShareableContentWidgetViewComponent : ViewComponent
    {
        public const string IDENTITY = "Generic.ShareableContentWidget";
        private readonly IPageRetriever _pageRetriever;
        private readonly ISiteRepository _siteRepository;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;

        public ShareableContentWidgetViewComponent(IPageRetriever pageRetriever, 
            ISiteRepository siteRepository, 
            ICacheDependenciesStore cacheDependenciesStore,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ICacheRepositoryContext cacheRepositoryContext)
        {
            _pageRetriever = pageRetriever;
            _siteRepository = siteRepository;
            _cacheDependenciesStore = cacheDependenciesStore;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _cacheRepositoryContext = cacheRepositoryContext;
        }


        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<ShareableContentWidgetProperties> widgetProperties)
        {
            // Not initialized
            if (widgetProperties == null)
            {
                return View("/Components/Widgets/ShareableContentWidget/ShareableContentWidget.cshtml", new ShareableContentWidgetComponentViewModel());
            }

            // Get DocumentID
            var docIDResult = await GetDocumentIDAsync(widgetProperties.Properties);
            if(!docIDResult.TryGetValue(out var docID))
            {
                return View("ShareableContentWidget", new ShareableContentWidgetComponentViewModel());
            }

            return View("/Components/Widgets/ShareableContentWidget/ShareableContentWidget.cshtml", new ShareableContentWidgetComponentViewModel()
            {
                WidgetProperties = widgetProperties.Properties,
                DocumentID = docID
            });
        }
        private async Task<Result<int>> GetDocumentIDAsync(ShareableContentWidgetProperties Properties)
        {
            if (Properties.Pages == null || !Properties.Pages.FirstOrMaybe().TryGetValue(out var page))
            {
                return Result.Failure<int>("No page selected");
            }
            var pageGUID = page.NodeGuid;
            var builder = _cacheDependencyBuilderFactory.Create()
                .Node(pageGUID);

            var foundPage = await _pageRetriever.RetrieveAsync<ShareableContentPageType>(
                    query => query
                        .WhereEquals(nameof(TreeNode.NodeGUID), pageGUID)
                        .WithCulturePreviewModeContext(_cacheRepositoryContext)
                        .Columns(nameof(TreeNode.DocumentID)),
                    cs => cs.Configure(builder, CacheMinuteTypes.Long.ToDouble(), "ShareableContentWidgetGetDocumentID", pageGUID)
                );
            return Result.SuccessIf(foundPage.Any(), foundPage.First().DocumentID, "No document found");
        }
    }

    public class ShareableContentWidgetProperties : IWidgetProperties
    {
        [EditingComponent(PageSelector.IDENTIFIER, Label = "Page", Order = 1, Tooltip = "Select a Shareable Content Page to render")]
        public IEnumerable<PageSelectorItem> Pages { get; set; } = Array.Empty<PageSelectorItem>();
    }

    public record ShareableContentWidgetComponentViewModel
    {
        public ShareableContentWidgetComponentViewModel() { }
        public ShareableContentWidgetComponentViewModel(int documentID)
        {
            DocumentID = documentID;
        }

        public Maybe<int> DocumentID { get; set; }
        public Maybe<ShareableContentWidgetProperties> WidgetProperties { get; set; }
    }


}
