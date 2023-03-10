using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;

namespace Core.Middleware
{
    public static class CustomVaryByMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomVaryByHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomVaryByHeaders>();
        }
    }

    /// <summary>
    /// Injects custom header-parameters to aid in custom vary-by-header logic
    /// </summary>
    public class CustomVaryByHeaders
    {
        public const string _CULTURE = "x-culture";
        public const string _SITE = "x-site";

        //public const string _SOMETHING = "x-something";
        //public const string _SOMETHING_ELSE = "x-something-else";

        private readonly RequestDelegate _next;


        /// <summary>
        /// Use this in the <cache vary-by-header=@CustomVaryByHeaders.CultureSiteVaryBy() ></cache> to vary by site
        /// </summary>
        /// <returns></returns>
        public static string SiteVaryBy()
        {
            return _SITE;
        }

        /// <summary>
        /// Use this in the <cache vary-by-header=@CustomVaryByHeaders.CultureSiteVaryBy() ></cache> to vary by culture and site
        /// </summary>
        /// <returns></returns>
        public static string CultureSiteVaryBy()
        {
            return $"{_CULTURE},{_SITE}";
        }


        /// <summary>
        /// Use this in the <cache vary-by-header=@CustomVaryByHeaders.CultureSiteVaryBy() ></cache> to vary by culture
        /// </summary>
        /// <returns></returns>
        public static string CultureVaryBy()
        {
            return $"{_CULTURE}";
        }

        public CustomVaryByHeaders(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add custom headers here, then in your caching you can use <cache vary-by-header=@($"{CustomVaryByHeaders._SOMETHING},{CustomVaryByHeaders._SOMETHING_ELSE}")
            //context.Request.Headers.Add(_SOMETHING, "some value");
            //context.Request.Headers.Add(_SOMETHING_ELSE, "some other value");

            var _siteRepository = context.RequestServices.GetRequiredService<ISiteRepository>();

            // Add custom headers here, then in your caching you can use <cache vary-by-header=@($"{CustomVaryByHeaders._SOMETHING},{CustomVaryByHeaders._SOMETHING_ELSE}")
            context.Request.Headers.AddOrReplace(_CULTURE, (CultureInfo.DefaultThreadCurrentCulture?.Name ?? "en-US").Split('-')[0]);
            context.Request.Headers.AddOrReplace(_SITE, _siteRepository.CurrentSiteName());

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}