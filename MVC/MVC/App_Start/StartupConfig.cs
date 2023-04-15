using Kentico.Activities.Web.Mvc;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Scheduler.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RelationshipsExtended.Interfaces;
using BootstrapLayoutTool;
using PageBuilderContainers;
using PageBuilderContainers.Base;
using PartialWidgetPage;
using Kentico.Membership;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using CMS.Helpers;
using System.Reflection;
using XperienceCommunity.Authorization;
using XperienceCommunity.Localizer;
using XperienceCommunity.PageBuilderUtilities;
using FluentValidation.AspNetCore;
using XperienceCommunity.WidgetFilter;
using Core.Middleware;
using Core;

namespace MVC
{
    public static class StartupConfig
    {

        public static void RegisterInterfaces(IServiceCollection services, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            // MVC Caching
            services.AddMVCCaching();
            services.AddMVCCachingAutoDependencyInjectionByAttribute();

            // Baseline services
            services.UseCoreBaseline();

            // Relationships Extended
            services.AddSingleton<IRelationshipExtendedHelper, RelationshipsExtendedHelper>();

            // Page Builder Container
            // services.AddSingleton<IPageBuilderContainerHelper, PageBuilderContainerHelper>();

            // Partial Widget Page
            // services.AddSingleton<IPartialWidgetPageHelper, PartialWidgetPageHelper>();

            // Admin redirect filter
            services.AddSingleton<IStartupFilter>(new AdminRedirectStartupFilter(Configuration));

            // Environment tag helper
            services.AddSingleton<IPageBuilderContext, XperiencePageBuilderContext>();

            // Add up IUrlHelper
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            // Page template filters
            services.AddPageTemplateFilters(Assembly.GetExecutingAssembly());

            // Kentico authorization
            // services.AddKenticoAuthorization();

            // Fluent Validator
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(new Assembly[] { typeof(Startup).Assembly }));

            // Widget Filters
            services.AddWidgetFilter();
        }

        public static void RegisterKenticoServices(IServiceCollection services, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            // Enable desired Kentico Xperience features
            var kenticoServiceCollection = services.AddKentico(features =>
            {
                features.UsePageBuilder(new PageBuilderOptions()
                {
                    // Specifies a default section for the page builder feature
                    DefaultSectionIdentifier = Bootstrap4LayoutToolProperties.IDENTITY,
                    // Disables the system's built-in 'Default' section
                    RegisterDefaultSection = true
                });

                features.UsePageRouting(new PageRoutingOptions()
                {
                    EnableAlternativeUrls = true,
                    EnableRouting = true,
                    //CultureCodeRouteValuesKey = "culture"
                });

                // Data annotationslocationation?

                //Enable Campaign Tracking
                // features.UseCampaignLogger();

                //Allows the site to track automatic activities - External Search and Page View
                features.UseActivityTracking();

                //Allows tracking of email marketing activities
                /*features.UseEmailTracking(new EmailTrackingOptions()
                {
                    EmailLinkHandlerRouteUrl = CMS.Newsletters.EmailTrackingLinkHelper.DEFAULT_LINKS_TRACKING_ROUTE_HANDLER_URL,
                    OpenedEmailHandlerRouteUrl = CMS.Newsletters.EmailTrackingLinkHelper.DEFAULT_OPENED_EMAIL_TRACKING_ROUTE_HANDLER_URL
                });*/

                // features.UseABTesting();

                // features.UseWebAnalytics();

                features.UseScheduler();
            }).SetAdminCookiesSameSiteNone();

            // Add Other Services Here

            if (Environment.IsDevelopment())
            {
                // By default, Xperience sends cookies using SameSite=Lax. If the administration and live site applications
                // are hosted on separate domains, this ensures cookies are set with SameSite=None and Secure. The configuration
                // only applies when communicating with the Xperience administration via preview links. Both applications also need 
                // to use a secure connection (HTTPS) to ensure cookies are not rejected by the client.
                kenticoServiceCollection.SetAdminCookiesSameSiteNone();

                // By default, Xperience requires a secure connection (HTTPS) if administration and live site applications
                // are hosted on separate domains. This configuration simplifies the initial setup of the development
                // or evaluation environment without a the need for secure connection. The system ignores authentication
                // cookies and this information is taken from the URL.
                kenticoServiceCollection.DisableVirtualContextSecurityForLocalhost();
            }

            services.AddPageNavigationRedirects(options =>
            {
                /* Customize here of you wish */
            });
        }

        public static void AddAuthentication(IServiceCollection services, IConfiguration configuration, string AUTHENTICATION_COOKIE_NAME = "identity.authentication")
        {
            // Adds Basic Kentico Authentication, needed for user context and some tools
            services.AddCoreBaselineKenticoAuthentication(configuration, AUTHENTICATION_COOKIE_NAME);
        }
        internal static void RegisterGzipFileHandling(IServiceCollection services, IWebHostEnvironment environment, IConfiguration Configuration)
        {
            services.UseGzipAndCacheControlFileHandling();
        }

        public static void RegisterLocalizationAndControllerViews(IServiceCollection services, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            // Localizer
            services.AddLocalization()
                    .AddXperienceLocalizer() // Call after AddLocalization
                    .AddControllersWithViews() // .AddControllersWithViewsAndKenticoAuthorization()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                        {
                            return factory.Create(typeof(SharedResources));
                        };
                    });
        }

        

        public static void RegisterDotNetCoreConfigurationsAndKentico(IApplicationBuilder app, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
                app.UseHsts();
            }

            //////////////////////////////
            //////// ERROR HANDLING //////
            //////////////////////////////

            // Standard HttpError handling
            // See Features/HttpErrors/HttpErrorsController.cs
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            // BizStream's Status Code Pages
            // See Features/HttpErrors/XperienceStausCodePage.cs
            // app.UseXperienceStatusCodePages();

            //////////////////////////////
            //////// ERROR HANDLING //////
            //////////////////////////////


            app.UseStaticFiles();

            // While IIS and IIS Express automatically handle StaticFiles from the root, default Kestrel doesn't, so safer to 
            // add this for any Site Media Libraries if you ever plan on linking directly to the file.  /getmedia linkes are not
            // impacted. 
            //
            // Also, if you ever need to bypass the IIS/IIS Express default StaticFile handling, you can add a web.config in the media folder 
            // with the below:
            // <?xml version="1.0"?>
            // <configuration>
            //     <system.webServer>
            //         <handlers>
            //             <add name="ForceStaticFileHandlingToNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Either" requireAccess="Read" />
            //         </handlers>
            //     </system.webServer>
            // </configuration>
            /*
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Baseline")),
                    RequestPath = "/Baseline"
                });
            */
            app.UseStaticFiles();

            app.UseKentico();

            app.UseCookiePolicy();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCustomVaryByHeaders();

        }
    }
}
