using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Navigation.TagHelpers
{
    [HtmlTargetElement("li", Attributes = "[navitem-references]")]
    [HtmlTargetElement("a", Attributes = "[navitem-references]")]

    public class NavigationItemNavReferenceTagHelper : TagHelper
    {
        public NavigationItem? NavigationItem { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(NavigationItem == null)
            {
                return;
            }
            if(NavigationItem.LinkPagePath.TryGetValueNonEmpty(out var linkPagePathVal))
            {
                output.Attributes.Add("data-navpath", linkPagePathVal.ToLowerInvariant());
            }
            if (NavigationItem.LinkHref.TryGetValueNonEmpty(out var linkHrefVal))
            {
                output.Attributes.Add("data-navhref", linkHrefVal.ToLowerInvariant());
            }
        }
    }

    [HtmlTargetElement("li", Attributes = "[navitem-class]")]
    [HtmlTargetElement("article", Attributes = "[navitem-class]")]
    [HtmlTargetElement("a", Attributes = "[navitem-class]")]

    public class NavigationItemClassTagHelper : TagHelper
    {
        public NavigationItem? NavigationItem { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (NavigationItem == null)
            {
                return;
            }
            if (NavigationItem.LinkCSSClass.TryGetValueNonEmpty(out var linkCSSClassVal))
            {
                output.AddClass(linkCSSClassVal.ToLowerInvariant(), HtmlEncoder.Default);
            }
        }
    }

    [HtmlTargetElement("a", Attributes = "[navitem-link]")]

    public class NavigationItemLinkTagHelper : TagHelper
    {
        public NavigationItem? NavigationItem { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (NavigationItem == null)
            {
                return;
            }
            if(!output.Attributes.ContainsName("title") && NavigationItem.LinkAlt.TryGetValueNonEmpty(out var linkAltVal))
            {
                output.Attributes.Add("title", linkAltVal);
            }
            if (!output.Attributes.ContainsName("onclick") && NavigationItem.LinkOnClick.TryGetValueNonEmpty(out var linkOnClickVal))
            {
                output.Attributes.Add("onclick", linkOnClickVal);
            }
            if (!output.Attributes.ContainsName("href") && NavigationItem.LinkHref.TryGetValueNonEmpty(out var linkHrefVal))
            {
                output.Attributes.Add("href", linkHrefVal);
            }
        }
    }
}
