using Core.Services;
using Microsoft.AspNetCore.Http;
using MVCCaching;

namespace Core.Services.Implementations
{
    [AutoDependencyInjection]
    public class UrlResolver : IUrlResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetAbsoluteUrl(string relativeUrl)
        {
            return GetUri(ResolveUrl(relativeUrl)).AbsoluteUri;
        }

        public string ResolveUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }
            if (url.StartsWith("~/"))
            {
                url = url.Replace("~/", "/");
            }
            return url;
        }
        private Uri GetUri(string relativeUrl)
        {
            relativeUrl = !string.IsNullOrWhiteSpace(relativeUrl) ? relativeUrl : "/";
            if (_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext))
            {
                var request = httpContext.Request;
                return new UriBuilder
                {
                    Scheme = request.Scheme,
                    Host = request.Host.Value,
                    Path = relativeUrl.Split('?')[0],
                    Query = (relativeUrl.Contains('?') ? relativeUrl.Split('?')[1] : "")
                }.Uri;
            }
            else
            {
                return new Uri(relativeUrl);
            }
        }
    }
}
