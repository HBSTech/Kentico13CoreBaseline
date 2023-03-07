using Navigation.Features.PartialNavigation;

[assembly: RegisterPageTemplate(
    "Generic.Navigation_Default",
    "Navigation",
    typeof(NavigationPageTemplateProperties),
    "/Features/PartialNavigation/NavigationPageTemplate.cshtml")]

namespace Navigation.Features.PartialNavigation
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
