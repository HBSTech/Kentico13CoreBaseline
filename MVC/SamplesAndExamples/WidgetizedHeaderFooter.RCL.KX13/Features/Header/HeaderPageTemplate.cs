using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using WidgetizedHeaderFooter.Features.Header;

[assembly: RegisterPageTemplate(
    "Generic.Header_Default",
    "Header",
    typeof(HeaderPageTemplateProperties),
    "/Features/Header/HeaderPageTemplate.cshtml")]

namespace WidgetizedHeaderFooter.Features.Header
{
    public class HeaderPageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.Header";
    }

    public class HeaderPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}