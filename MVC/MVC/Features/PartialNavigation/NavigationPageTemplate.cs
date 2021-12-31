using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.PartialNavigation;

[assembly: RegisterPageTemplate(
    "Generic.Navigation_Default",
    "Navigation",
    typeof(NavigationPageTemplateProperties),
    "~/Features/PartialNavigation/NavigationPageTemplate.cshtml")]

namespace Generic.Features.PartialNavigation
{
    public class NavigationPageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.Navigation";
    }

    public class NavigationPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}
