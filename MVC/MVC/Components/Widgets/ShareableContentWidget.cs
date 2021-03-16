using CMS;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using CMS.Helpers;
using Generic;
using Generic.Models;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using MVCCaching.Base.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

[assembly: RegisterWidget(ShareableContentWidgetViewComponent.IDENTITY,
    typeof(ShareableContentWidgetViewComponent),
                         "Shareable Content",
    typeof(ShareableContentWidgetProperties),
                         Description = "Displays the widget content of a Shareable Content Page", IconClass = "icon-recaptcha", AllowCache = true)]
namespace Generic
{
    [CacheVaryBy(VaryByCulture = true)]
    public class ShareableContentWidgetViewComponent : ViewComponent
    {
        public const string IDENTITY = "Generic.ShareableContentWidget";

        public ShareableContentWidgetViewComponent(IPageRetriever pageRetriever, IProgressiveCache progressiveCache, ICacheDependenciesStore cacheDependenciesStore, ICacheDependenciesScope cacheDependenciesScope)
        {
            PageRetriever = pageRetriever;
            ProgressiveCache = progressiveCache;
            CacheDependenciesStore = cacheDependenciesStore;
            CacheDependenciesScope = cacheDependenciesScope;
        }

        public IPageRetriever PageRetriever { get; }
        public IProgressiveCache ProgressiveCache { get; }
        public ICacheDependenciesStore CacheDependenciesStore { get; }
        public ICacheDependenciesScope CacheDependenciesScope { get; }

        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<ShareableContentWidgetProperties> widgetProperties)
        {
            // Not initialized
            if (widgetProperties == null)
            {
                return View("~/Views/Shared/Widgets/_ShareableContentWidget.cshtml", new ShareableContentWidgetComponentViewModel()
                {
                    WidgetProperties = null,
                    DocumentID = 0
                });
            }

            // Get DocumentID
            int DocID = GetDocumentID(widgetProperties.Properties);
            // Theoretically, there could be more dependencies within the page rendering, so if that becomes the case for a project, may have to turn this 'off' as very hard to get the shareable content's widget dependencies.
            widgetProperties.CacheDependencies.CacheKeys = new string[] {
                $"documentid|{DocID}",
                $"documentid|{DocID}|attachments" };

            if(!string.IsNullOrWhiteSpace(widgetProperties.Properties.ContainerName))
            {
                widgetProperties.CacheDependencies.CacheKeys.Add($"{PageBuilderContainerInfo.OBJECT_TYPE}|byname|{widgetProperties.Properties.ContainerName}");
            }

            return View("~/Views/Shared/Widgets/_ShareableContentWidget.cshtml", new ShareableContentWidgetComponentViewModel()
            {
                WidgetProperties = widgetProperties.Properties,
                DocumentID = DocID
            });
        }
        private int GetDocumentID(ShareableContentWidgetProperties Properties)
        {
            if (Properties.Pages == null || !Properties.Pages.Any(x => true))
            {
                return 0;
            }
            string culture = !string.IsNullOrWhiteSpace(Properties.Culture) ? Properties.Culture : System.Globalization.CultureInfo.CurrentCulture.Name;
            return ProgressiveCache.Load(cs =>
            {
                var FoundPage = PageRetriever.Retrieve<ShareableContent>(query =>
                {
                    query.WhereEquals(nameof(TreeNode.NodeGUID), Properties.Pages.FirstOrDefault().NodeGuid)
                    .Culture(culture)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .Columns(nameof(TreeNode.DocumentID));
                }
                ).FirstOrDefault();
                return (FoundPage != null ? FoundPage.DocumentID : 0);
            }, new CacheSettings(1440, "GetShareableContentWidget", Properties.Pages.FirstOrDefault().NodeGuid.ToString(), culture));
        }
    }


}
