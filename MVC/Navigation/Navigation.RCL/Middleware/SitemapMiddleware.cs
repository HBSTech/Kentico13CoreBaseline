using Microsoft.Extensions.DependencyInjection;

namespace Navigation.Middleware
{
    public static class SitemapMiddleware
    {
        public static IServiceCollection UseSiteMap(this IServiceCollection services, SitemapConfiguration options)
        {
            /* Example site map options
                new SiteMapOptions()
                {
                    Path = "/MasterPage/Navigation",
                    ClassNames = new string[] { "Generic.Navigation" },
                    UrlColumnName = "NavigationLinkUrl",
                    CacheItemName = "SiteMap"
                }
                new SiteMapOptions()
                {
                    Path = "/%",
                    ClassNames = new string[] { "Generic.BasicPage" },
                    CacheItemName = "SiteMapPages"
                }
            */

            return services.AddSingleton(options);
        }
    }
}
