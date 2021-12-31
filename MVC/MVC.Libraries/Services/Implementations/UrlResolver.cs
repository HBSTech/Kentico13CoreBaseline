using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace Generic.Services.Implementations
{
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
            if(url.StartsWith("~/"))
            {
                url = url.Replace("~/", "/");
            }
            return url;
        }
        private Uri GetUri(string relativeUrl)
        {
            relativeUrl = !string.IsNullOrWhiteSpace(relativeUrl) ? relativeUrl : "/";
            var request = _httpContextAccessor.HttpContext.Request;
            return new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Value,
                Path = relativeUrl.Split('?')[0],
                Query = (relativeUrl.Contains('?') ? relativeUrl.Split('?')[1] : "")
            }.Uri;   
        }
    }
}
