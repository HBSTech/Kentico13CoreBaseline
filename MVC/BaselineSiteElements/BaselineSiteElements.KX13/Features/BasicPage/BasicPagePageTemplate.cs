using BaselineSiteElements.Features.BasicPage;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate(
    "Generic.BasicPage_Default",
    //"Generic.GenericPage_Default",
    "Basic Page",
    typeof(BasicPagePageTemplateProperties),
    "~/Features/BasicPage/BasicPagePageTemplate.cshtml")]

namespace BaselineSiteElements.Features.BasicPage
{
    public class BasicPagePageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.BasicPage";
        //public override string PageTypeClassName => "Generic.GenericPage";
    }

    public class BasicPagePageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}