using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Generic.Library
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
        //public const string _SOMETHING = "x-something";
        //public const string _SOMETHING_ELSE = "x-something-else";

        private readonly RequestDelegate _next;

        public CustomVaryByHeaders(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add custom headers here, then in your caching you can use <cache vary-by-header=@($"{CustomVaryByHeaders._SOMETHING},{CustomVaryByHeaders._SOMETHING_ELSE}")
            //context.Request.Headers.Add(_SOMETHING, "some value");
            //context.Request.Headers.Add(_SOMETHING_ELSE, "some other value");

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}