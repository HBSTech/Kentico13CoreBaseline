using CMS;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using CMS.Helpers;
using Generic.Components.Widgets.ShareableContentWidget;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Content.Web.Mvc;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using MVCCaching.Base.Core.Interfaces;
using PageBuilderContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: RegisterWidget(ShareableContentWidgetViewComponent.IDENTITY,
    typeof(ShareableContentWidgetViewComponent),
                         "Shareable Content",
    typeof(ShareableContentWidgetProperties),
                         Description = "Displays the widget content of a Shareable Content Page", IconClass = "icon-recaptcha", AllowCache = true)]
namespace Generic.Components.Widgets.ShareableContentWidget
{
    [CacheVaryBy(VaryByCulture = true)]
    public class ShareableContentWidgetViewComponent : ViewComponent
    {
        public const string IDENTITY = "Generic.ShareableContentWidget";
        private readonly IPageRetriever _pageRetriever;

        public ShareableContentWidgetViewComponent(IPageRetriever pageRetriever)
        {
            _pageRetriever = pageRetriever;
        }


        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<ShareableContentWidgetProperties> widgetProperties)
        {
            // Not initialized
            if (widgetProperties == null)
            {
                return View("ShareableContentWidget", new ShareableContentWidgetComponentViewModel()
                {
                    WidgetProperties = null,
                    DocumentID = 0
                });
            }

            // Get DocumentID
            int docID = await GetDocumentIDAsync(widgetProperties.Properties);
            if(docID == 0)
            {
                return View("ShareableContentWidget", new ShareableContentWidgetComponentViewModel()
                {
                    WidgetProperties = null,
                    DocumentID = 0
                });
            }

            // Theoretically, there could be more dependencies within the page rendering, so if that becomes the case for a project, may have to turn this 'off' as very hard to get the shareable content's widget dependencies.
            widgetProperties.CacheDependencies.CacheKeys = new string[] {
                $"documentid|{docID}",
                $"documentid|{docID}|attachments" };

            if (!string.IsNullOrWhiteSpace(widgetProperties.Properties.ContainerName))
            {
                widgetProperties.CacheDependencies.CacheKeys.Add($"{PageBuilderContainerInfo.OBJECT_TYPE}|byname|{widgetProperties.Properties.ContainerName}");
            }

            return View("ShareableContentWidget", new ShareableContentWidgetComponentViewModel()
            {
                WidgetProperties = widgetProperties.Properties,
                DocumentID = docID
            });
        }
        private async Task<int> GetDocumentIDAsync(ShareableContentWidgetProperties Properties)
        {
            if (Properties.Pages == null || !Properties.Pages.Any(x => true))
            {
                return 0;
            }
            var pageGUID = Properties.Pages.FirstOrDefault().NodeGuid;

            string culture = !string.IsNullOrWhiteSpace(Properties.Culture) ? Properties.Culture : System.Globalization.CultureInfo.CurrentCulture.Name;
            var foundPage = await _pageRetriever.RetrieveAsync<ShareableContent>(
                    query => query
                        .WhereEquals(nameof(TreeNode.NodeGUID), pageGUID)
                        .Culture(culture)
                        .CombineWithDefaultCulture()
                        .CombineWithAnyCulture()
                        .Columns(nameof(TreeNode.DocumentID)),
                    cacheSettings => cacheSettings
                        .Dependencies((items, csbuilder) => csbuilder.Custom($"nodeguid|{pageGUID}"))
                        .Key($"ShareableContentWidgetGetDocumentID|{pageGUID}")
                        .Expiration(TimeSpan.FromMinutes(60))
                );
            return foundPage.FirstOrDefault()?.DocumentID ?? 0;
        }
    }

    public class ShareableContentWidgetProperties : PageBuilderWithHtmlBeforeAfterSectionProperties, IWidgetProperties
    {

        [EditingComponent(PageSelector.IDENTIFIER, Label = "Page", Order = 1, Tooltip = "Select a Shareable Content Page to render")]
        public IEnumerable<PageSelectorItem> Pages { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Culture", ExplanationText = "Culture code, uses current culture otherwise", Order = 2)]
        public string Culture { get; set; }

    }

    public record ShareableContentWidgetComponentViewModel
    {
        public int DocumentID { get; set; }
        public ShareableContentWidgetProperties WidgetProperties { get; set; }
    }


}
