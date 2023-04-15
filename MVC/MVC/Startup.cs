using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;
using Core.Middleware;

namespace MVC
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            StartupConfig.RegisterInterfaces(services, Environment, Configuration);

            StartupConfig.RegisterKenticoServices(services, Environment, Configuration);

            StartupConfig.RegisterGzipFileHandling(services, Environment, Configuration);

            StartupConfig.RegisterLocalizationAndControllerViews(services, Environment, Configuration);

            StartupConfig.AddAuthentication(services, Configuration);

            services.AddHttpContextAccessor();

            // Feature folders
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomLocationExpander());
            });

            // Enable the blow for BizStrema's StatusCodePages
            // See Features/HttpErrors/XperienceStatusCodePages.cs for more instructions
            // services.AddXperienceStatusCodePages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            StartupConfig.RegisterDotNetCoreConfigurationsAndKentico(app, Environment, Configuration);

            RouteConfig.RegisterRoutes(app, Environment, Configuration);
        }
    }
}
