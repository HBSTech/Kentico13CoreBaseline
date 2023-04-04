using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Kentico.Web.Mvc;

namespace MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IApplicationBuilder app, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.Kentico().MapRoutes();

                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                   name: "error",
                   pattern: "error/{code}",
                   defaults: new { controller = "HttpErrors", action = "Error" }
                );

                //Site map
                endpoints.MapControllerRoute(
                    name: "MySiteMap",
                    pattern: "sitemap.xml",
                    defaults: new { controller = "Sitemap", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "MySiteMap_Google",
                    pattern: "googlesitemap.xml",
                    defaults: new { controller = "Sitemap", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
