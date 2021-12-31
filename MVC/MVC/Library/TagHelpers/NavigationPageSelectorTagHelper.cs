using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Generic.Library.TagHelpers
{
    [HtmlTargetElement("navigation-page-selector")]
    public class NavigationPageSelectorTagHelper : TagHelper
    {
        private readonly IPageContextRepository _pageContextRepository;
        private readonly IUrlResolver _urlResolver;

        public string ParentClass { get; set; }
        public string CurrentPagePath { get; set; }

        public NavigationPageSelectorTagHelper(IPageContextRepository pageContextRepository,
            IUrlResolver urlResolver)
        {
            _pageContextRepository = pageContextRepository;
            _urlResolver = urlResolver;
        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(CurrentPagePath))
            {
                if (CurrentPagePath == "/Home")
                {
                    // Special case for Home since the Link is just "/"
                    CurrentPagePath = "/";
                }
            }
            else
            {
                var currentPage = await _pageContextRepository.GetCurrentPageAsync();
                if (currentPage != null)
                {
                    CurrentPagePath = _urlResolver.ResolveUrl(currentPage.RelativeUrl);
                }
            }

            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("type", "text/javascript");
            string Javascript = "" +
                $"var elem = document.querySelector(\".{ParentClass} li[data-navhref='{CurrentPagePath?.ToLower()}']\");" +
                $"elem = elem || document.querySelector(\".{ParentClass} li[data-navpath='{CurrentPagePath?.ToLower()}']\");" +
                "if (elem)" +
                "{" +
                "   elem.classList.add('active');" +
                "}";
            output.Content.SetHtmlContent(Javascript);
        }

    }
}
