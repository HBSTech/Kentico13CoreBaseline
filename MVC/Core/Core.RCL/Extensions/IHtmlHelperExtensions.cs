using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    public static class IHtmlHelperExtensions
    {
        public static IHtmlContent RawWithWrapper(this IHtmlHelper helper, string html, string wrapperElementTag)
        {
            if (!html.Trim().StartsWith('<'))
            {
                return helper.Raw($"<{wrapperElementTag}>{html}</{wrapperElementTag}>");
            }
            return helper.Raw(html);
        }

        public static string AddFileVersionToPath(this IHtmlHelper helper, string path)
        {
            var fileVersion = helper.ViewContext.HttpContext.RequestServices.GetService<IFileVersionProvider>();
            if (fileVersion != null)
            {
                return fileVersion.AddFileVersionToPath(helper.ViewContext.HttpContext.Request.Path, path);
            }
            else
            {
                return path;
            }

        }
    }
}
