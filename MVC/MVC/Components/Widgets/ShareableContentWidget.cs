using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using CMS.Helpers;
using Generic;
using Generic.Models;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[assembly: RegisterWidget(ShareableContentWidgetViewComponent.IDENTITY,
    typeof(ShareableContentWidgetViewComponent),
                         "Shareable Content",
    typeof(ShareableContentWidgetProperties),
                         Description = "Displays the widget content of a Shareable Content Page", IconClass = "icon-recaptcha")]
namespace Generic
{
    public class ShareableContentWidgetViewComponent : ViewComponent
    {
        public const string IDENTITY = "Generic.ShareableContentWidget";

        public ShareableContentWidgetViewComponent(IPageRetriever pageRetriever, IProgressiveCache progressiveCache)
        {
            PageRetriever = pageRetriever;
            ProgressiveCache = progressiveCache;
        }

        public IPageRetriever PageRetriever { get; }
        public IProgressiveCache ProgressiveCache { get; }

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
            return View("~/Views/Shared/Widgets/_ShareableContentWidget.cshtml", new ShareableContentWidgetComponentViewModel()
            {
                WidgetProperties = widgetProperties.Properties,
                DocumentID = GetDocumentID(widgetProperties.Properties)
            });
        }
        private int GetDocumentID(ShareableContentWidgetProperties Properties)
        {
            if (Properties.Pages == null || Properties.Pages.Count() == 0)
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
