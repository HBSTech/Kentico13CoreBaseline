using Kentico.Activities.Web.Mvc;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Scheduler.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RelationshipsExtended.Interfaces;
using RelationshipsExtended;
using BootstrapLayoutTool;
using PageBuilderContainers;
using PageBuilderContainers.Base;
using PartialWidgetPage;
using XperienceCommunity.PageBuilderTagHelpers;
using Kentico.Membership;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using CMS.Helpers;
using System;
using Generic.Library;
using Generic.Repositories.Implementations;
using System.Reflection;
using XperienceCommunity.Authorization;
using XperienceCommunity.Localizer;
using XperienceCommunity.PageBuilderUtilities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;
using Generic.Repositories.Interfaces;
using Core.Features.Account.LogIn;
using Generic.Repositories.Implementation;
using System.Collections.Generic;
using XperienceCommunity.WidgetFilter;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Generic.App_Start
{
    public static class StartupConfig
    {
        private const string AUTHENTICATION_COOKIE_NAME = "identity.authentication";

        public static void RegisterInterfaces(IServiceCollection services, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            // Relationships Extended
            services.AddSingleton<IRelationshipExtendedHelper, RelationshipsExtendedHelper>();

            // Page Builder Container
            services.AddSingleton<IPageBuilderContainerHelper, PageBuilderContainerHelper>();

            // Partial Widget Page
            services.AddSingleton<IPartialWidgetPageHelper, PartialWidgetPageHelper>();

            // Custom PartialWidgetRenderingRetriever
            services.AddSingleton<IPartialWidgetRenderingRetriever, PartialWidgetRenderingRetriever>();

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

            // AutoMapper of MVC and MVC.Libraries solutions
            services.AddAutoMapper(typeof(Startup), typeof(Generic.Libraries.AssemblyInfo));

            // Kentico authorization
            services.AddKenticoAuthorization();

            // Fluent Validator
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(new Assembly[] { typeof(Startup).Assembly, typeof(Generic.Libraries.AssemblyInfo).Assembly, typeof(Generic.Models.AssemblyInfo).Assembly }));

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

        internal static void RegisterGzipFileHandling(IServiceCollection services, IWebHostEnvironment environment, IConfiguration Configuration)
        {
            services.ConfigureOptions<GzipStaticFileOptions>();
        }

        public static void RegisterLocalizationAndControllerViews(IServiceCollection services, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            // From dancing goat, Localizer

            services.AddLocalization()
                    .AddXperienceLocalizer() // Call after AddLocalization
                    .AddControllersWithViews(options => options.Filters.AddKenticoAuthorization())
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                        {
                            return factory.Create(typeof(SharedResources));
                        };
                    });
        }

        public static void RegisterIdentityHandlers(IServiceCollection services, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            // Required for authentication
            services.AddScoped<IPasswordHasher<ApplicationUser>, Kentico.Membership.PasswordHasher<ApplicationUser>>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddApplicationIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Note: These settings are effective only when password policies are turned off in the administration settings.
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
                    .AddApplicationDefaultTokenProviders()
                    .AddUserStore<ApplicationUserStore<ApplicationUser>>()
                    .AddRoleStore<ApplicationRoleStore<ApplicationRole>>()
                    .AddUserManager<ApplicationUserManager<ApplicationUser>>()
                    .AddSignInManager<SignInManager<ApplicationUser>>();
            // Get default 

            services.AddAuthentication()
                // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-6.0&tabs=visual-studio
                /*.AddGoogle("Google", opt =>
                {
                    var googleAuth = Configuration.GetSection("Authentication:Google");

                    opt.ClientId = googleAuth["ClientId"];
                    opt.ClientSecret = googleAuth["ClientSecret"];
                    opt.SignInScheme = IdentityConstants.ExternalScheme;
                    opt.EventsType = typeof(SiteSettingsOauthAuthenticationEvents);
                }).AddFacebook("Facebook", opt =>
                {
                    var facebookAuth = Configuration.GetSection("Authentication:Facebook");

                    opt.AppId = facebookAuth["AppId"];
                    opt.AppSecret = facebookAuth["AppSecret"];
                    opt.SignInScheme = IdentityConstants.ExternalScheme;
                    opt.EventsType = typeof(SiteSettingsFacebookOauthAuthenticationEvents);
                }).AddTwitter(opt =>
                {
                    var twitterAuth = Configuration.GetSection("Authentication:Twitter");

                    opt.ConsumerKey = twitterAuth["APIKey"];
                    opt.ConsumerSecret = twitterAuth["APIKeySecret"];
                    opt.RetrieveUserDetails = true;
                    opt.EventsType = typeof(SiteSettingsTwitterOauthAuthenticationEvents);
                }).AddMicrosoftAccount(opt =>
                {
                    var microsoftAuth = Configuration.GetSection("Authentication:Microsoft");

                    opt.ClientId = microsoftAuth["ClientId"];
                    opt.ClientSecret = microsoftAuth["ClientSecret"];
                    opt.EventsType = typeof(SiteSettingsOauthAuthenticationEvents);
                })*/
                // Baseline Configuration of External Authentication
                .ConfigureAuthentication(config =>
                {
                    config.ExistingInternalUserBehavior = Models.Account.ExistingInternalUserBehavior.SetToExternal;
                    config.FacebookUserRoles.Add("facebook-user");
                    config.UseTwoFormAuthentication = false;
                });
            services.AddAuthorization();

            // Register authentication cookie
            // Overwrite login logout based on site settings, with fall back to the default controllers
            services.AddAuthenticationServices();

            // Configures the application's authentication cookie
            services.ConfigureApplicationCookie(c =>
            {
                // These 3 are actually handled on the SiteSettingsOauthAuthenticationEvents
                // and are overwritten by site settings
                c.LoginPath = new PathString("/Account/Signin");
                c.LogoutPath = new PathString("/Account/Signout");
                c.AccessDeniedPath = new PathString("/Error/403");
                
                c.ExpireTimeSpan = TimeSpan.FromDays(14);
                c.SlidingExpiration = true;
                c.Cookie.Name = AUTHENTICATION_COOKIE_NAME;
                c.EventsType = typeof(SiteSettingsCookieAuthenticationEvents);
            });


            CookieHelper.RegisterCookie(AUTHENTICATION_COOKIE_NAME, CookieLevel.Essential);
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
