using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TabbedPages.Features.Tab;

[assembly: RegisterPageTemplate(
    "Generic.Tab_Default",
    "Tab",
    typeof(TabPageTemplateProperties),
    "/Features/Tab/TabPageTemplate.cshtml")]

namespace TabbedPages.Features.Tab
{
    public class TabPageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.Tab";
    }

    public class TabPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}