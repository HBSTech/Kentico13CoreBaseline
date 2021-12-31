using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.Tab;

[assembly: RegisterPageTemplate(
    "Generic.Tab_Default",
    "Tab",
    typeof(TabPageTemplateProperties),
    "~/Features/Tab/TabPageTemplate.cshtml")]

namespace Generic.Features.Tab
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