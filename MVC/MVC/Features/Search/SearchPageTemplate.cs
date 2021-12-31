using Generic.Features.Search;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate(
    "Generic.Search_Default",
    "Search",
    typeof(SearchPageTemplateProperties),
    "~/Features/Search/SearchPageTemplate.cshtml")]

namespace Generic.Features.Search
{
    public class SearchPageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.Search";
    }

    public class SearchPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}