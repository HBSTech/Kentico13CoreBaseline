using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Core
{
    [HtmlTargetElement("*", Attributes = "[bl-link-wrapper]")]
    public class ILinkWrapperTagHelper : TagHelper
    {
        private readonly ILinkItemService _linkItemService;

        public ILinkWrapperTagHelper(ILinkItemService linkItemService)
        {
            _linkItemService = linkItemService;
        }

        public ILinkItem? blLinkItem { get; set; }
        public bool blLinkWrapInterior { get; set; } = false;
        public bool blLinkWrapOnlyIfGeneral { get; set; } = false;
        public string blLinkTitle { get; set; } = string.Empty;
        public string blLinkCss { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (blLinkItem != null)
            {
                if (blLinkWrapOnlyIfGeneral && blLinkItem is not GeneralLink)
                {
                    return;
                }
                string? linkTitle = string.IsNullOrWhiteSpace(blLinkTitle) ? null : blLinkTitle;
                string? linkCss = string.IsNullOrWhiteSpace(blLinkCss) ? null : blLinkCss;
                var linkHtml = _linkItemService.GetLinkBeginningEnd(blLinkItem, linkTitle, linkCss);

                if (blLinkWrapInterior)
                {
                    output.PreContent.AppendHtml(linkHtml.Item1);
                    output.PostContent.AppendHtml(linkHtml.Item2);
                }
                else
                {
                    output.PreElement.AppendHtml(linkHtml.Item1);
                    output.PostElement.AppendHtml(linkHtml.Item2);
                }
            }
        }

    }
}
