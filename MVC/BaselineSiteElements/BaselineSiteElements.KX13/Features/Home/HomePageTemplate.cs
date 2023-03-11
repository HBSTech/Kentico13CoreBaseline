using BaselineSiteElements.Features.Home;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate(
    "Generic.Home_Default",
    "Home Page (Default)",
    typeof(HomePageTemplateProperties),
    "~/Features/Home/HomePageTemplate.cshtml")]

namespace BaselineSiteElements.Features.Home
{
    public class HomePageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.Home";
    }

    public class HomePageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}