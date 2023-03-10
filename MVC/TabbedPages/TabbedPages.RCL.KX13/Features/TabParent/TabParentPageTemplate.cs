using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TabbedPages.Features.TabParent;

[assembly: RegisterPageTemplate(
    "Generic.TabParent_Default",
    "Tab Parent",
    typeof(TabParentPageTemplateProperties),
    "/Features/TabParent/TabParentPageTemplate.cshtml")]

namespace TabbedPages.Features.TabParent
{
    public class TabParentPageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.TabParent";
    }

    public class TabParentPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}