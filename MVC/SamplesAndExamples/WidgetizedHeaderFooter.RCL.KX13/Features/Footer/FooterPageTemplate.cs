using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using WidgetizedHeaderFooter.Features.Footer;

[assembly: RegisterPageTemplate(
    "Generic.Footer_Default",
    "Footer",
    typeof(FooterPageTemplateProperties),
    "/Features/Footer/FooterPageTemplate.cshtml")]

namespace WidgetizedHeaderFooter.Features.Footer
{
    public class FooterPageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.Footer";
    }

    public class FooterPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}