using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Core.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "optimize")]
    public class OptimizedPictureTagHelper : TagHelper
    {
        private string[] OptimizedImageExtensions = new string[] { "jpg", "jpeg", "png" };
        private string[] WebpImageExtensions = new string[] { "jpg", "jpeg" };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(output.Attributes.TryGetAttribute("src", out TagHelperAttribute attribute) && (attribute.Value != null && attribute.Value is string attrStrVal))
            {
                string imgSrc = "/"+ attrStrVal.Trim('/');

                if (imgSrc.StartsWith("/images/src", StringComparison.OrdinalIgnoreCase))
                {
                    string imageExtension = System.IO.Path.GetExtension(imgSrc).Trim('.');
                    bool hasOptimized = OptimizedImageExtensions.Contains(imageExtension, StringComparer.OrdinalIgnoreCase);
                    bool hasWebp = WebpImageExtensions.Contains(imageExtension, StringComparer.OrdinalIgnoreCase);

                    if (hasOptimized || hasWebp)
                    {
                        output.PreElement.AppendHtml("<picture>");

                        // use WebP
                        if (hasWebp)
                        {
                            output.PreElement.AppendHtml($"<source srcset=\"{imgSrc.Replace("/src/", "/webp/", StringComparison.OrdinalIgnoreCase).Replace(imageExtension, "webp", StringComparison.OrdinalIgnoreCase)}\" type=\"image/webp\"/>");
                        }
                        if(hasOptimized)
                        {
                            output.PreElement.AppendHtml($"<source srcset=\"{imgSrc.Replace("/src/", "/optimized/", StringComparison.OrdinalIgnoreCase)}\" type=\"image/{imageExtension.Replace("jpg", "jpeg", StringComparison.OrdinalIgnoreCase)}\"/>");
                        }                        
                        // normal image tag will appear here


                        output.PostElement.AppendHtml("</picture>");
                    }
                }
            }
            

            base.Process(context, output);
        }
    }
}
