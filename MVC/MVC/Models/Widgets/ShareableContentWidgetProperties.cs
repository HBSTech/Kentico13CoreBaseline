using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using PageBuilderContainers;
using System.Collections.Generic;

namespace Generic.Models
{
    public class ShareableContentWidgetProperties : PageBuilderWithHtmlBeforeAfterSectionProperties, IWidgetProperties
    {

        [EditingComponent(PageSelector.IDENTIFIER, Label = "Page", Order = 1, Tooltip = "Select a Shareable Content Page to render")]
        public IEnumerable<PageSelectorItem> Pages { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Culture", ExplanationText = "Culture code, uses current culture otherwise", Order = 2)]
        public string Culture { get; set; }

    }
}
