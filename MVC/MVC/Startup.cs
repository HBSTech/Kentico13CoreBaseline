using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using MVCCaching.Kentico;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Generic.App_Start;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Generic
{
    public class Startup
    {
        private const string AUTHENTICATION_COOKIE_NAME = "identity.authentication";

        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
            // MVC Caching
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.ConfigurationRoot = builder.Build();
            // END MVC Caching
        }

        public IConfigurationRoot ConfigurationRoot { get; private set; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // MVC Caching
        public void ConfigureContainer(ContainerBuilder builder)
        {
            DependencyResolverConfig.Register(builder, new Assembly[] { Assembly.GetExecutingAssembly(), typeof(Generic.Models.AssemblyInfo).Assembly, typeof(Generic.Libraries.AssemblyInfo).Assembly });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            StartupConfig.RegisterInterfaces(services, Environment, Configuration);

            StartupConfig.RegisterKenticoServices(services, Environment, Configuration);

            StartupConfig.RegisterGzipFileHandling(services, Environment, Configuration);

            StartupConfig.RegisterLocalizationAndControllerViews(services, Environment, Configuration);

            StartupConfig.RegisterIdentityHandlers(services, Environment, Configuration);

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
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            StartupConfig.RegisterDotNetCoreConfigurationsAndKentico(app, Environment, Configuration);

            RouteConfig.RegisterRoutes(app, Environment, Configuration);
        }
    }
}
