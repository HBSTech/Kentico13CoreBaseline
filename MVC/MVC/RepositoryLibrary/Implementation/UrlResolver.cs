using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Generic.Repositories.Implementations
{
    public class UrlResolver : IUrlResolver
    {

        public UrlResolver(IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        {
            UrlHelper = urlHelper;
            HttpContextAccessor = httpContextAccessor;
        }

        public IUrlHelper UrlHelper { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        public string GetAbsoluteUrl(string RelativeUrl)
        {
            return GetUri(ResolveUrl(RelativeUrl)).AbsoluteUri;
        }

        public string ResolveUrl(string Url)
        {
            return UrlHelper.Content(Url);
        }
        private Uri GetUri(string RelativeUrl)
        {
            RelativeUrl = !string.IsNullOrWhiteSpace(RelativeUrl) ? RelativeUrl : "/";
            var request = HttpContextAccessor.HttpContext.Request;
            return new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Value,
                Path = RelativeUrl.Split('?')[0],
                Query = (RelativeUrl.Contains('?') ? RelativeUrl.Split('?')[1] : "")
            }.Uri;
            
        }
    }
}
