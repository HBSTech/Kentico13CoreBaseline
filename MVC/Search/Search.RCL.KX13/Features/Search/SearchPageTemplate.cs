using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Search.Features.Search;

[assembly: RegisterPageTemplate(
    "Generic.Search_Default",
    "Search",
    typeof(SearchPageTemplateProperties),
    "~/Features/Search/SearchPageTemplate.cshtml")]

namespace Search.Features.Search
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