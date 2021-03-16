using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Library.TagHelpers
{
    [HtmlTargetElement("navigation-page-selector")]
    public class NavigationPageSelectorTagHelper : TagHelper
    {
        public string ParentClass { get; set; }
        public string CurrentPagePath { get; set; }
        public IPageDataContextRetriever DataRetriever { get; }
        public IUrlHelper UrlHelper { get; }

        public NavigationPageSelectorTagHelper(IPageDataContextRetriever dataRetriever, IUrlHelper urlHelper)
        {
            DataRetriever = dataRetriever;
            UrlHelper = urlHelper;
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
                if (DataRetriever.TryRetrieve(out IPageDataContext<TreeNode> Context))
                {
                    CurrentPagePath = UrlHelper.Content(DocumentURLProvider.GetUrl(Context.Page));
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
