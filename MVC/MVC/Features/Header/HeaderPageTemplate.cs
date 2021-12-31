using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.Header;

[assembly: RegisterPageTemplate(
    "Generic.Header_Default",
    "Header",
    typeof(HeaderPageTemplateProperties),
    "~/Features/Header/HeaderPageTemplate.cshtml")]

namespace Generic.Features.Header
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