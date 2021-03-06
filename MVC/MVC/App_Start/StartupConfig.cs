﻿using Controllers;
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
using XperienceCommunity.PageBuilderModeTagHelper;
using Kentico.Membership;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using CMS.Helpers;
using System;
using Generic.Library;

namespace Generic
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
            services.AddSingleton<IPartialWidgetRenderingRetriever, CustomPartialWidgetRenderingRetriever>();

            // Admin redirect filter
            services.AddSingleton<IStartupFilter>(new AdminRedirectStartupFilter(Configuration));

            // Environment tag helper
            services.AddSingleton<IPageBuilderContext, XperiencePageBuilderContext>();

            // Add up IUrlHelper
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(x => {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });


            // AutoMapper
            services.AddAutoMapper(typeof(Startup));
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
        }

        internal static void RegisterGzipFileHandling(IServiceCollection services, IWebHostEnvironment environment, IConfiguration Configuration)
        {
            services.ConfigureOptions<GzipStaticFileOptions>();
        }

        public static void RegisterLocalization(IServiceCollection services, IWebHostEnvironment Environment, IConfiguration Configuration)
        {
            // From dancing goat, Localizer
            services.AddLocalization()
                    .AddControllersWithViews()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
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

            services.AddAuthentication();
            services.AddAuthorization();

            // Register authentication cookie
            // Configures the application's authentication cookie
            services.ConfigureApplicationCookie(c =>
            {
                c.LoginPath = new PathString("/Account/Signin");
                c.AccessDeniedPath = new PathString("/Error/403");
                c.LogoutPath = new PathString("/Account/Signout");
                c.ExpireTimeSpan = TimeSpan.FromDays(14);
                c.SlidingExpiration = true;
                c.Cookie.Name = AUTHENTICATION_COOKIE_NAME;
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

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            /*app.UseStaticFiles(new StaticFileOptions
             {
                 OnPrepareResponse = context =>
                 {
                     IHeaderDictionary headers = context.Context.Response.Headers;
                     string contentType = headers["Content-Type"];
                     if (contentType == "application/x-gzip")
                     {
                         if (context.File.Name.EndsWith("js.gz"))
                         {
                             contentType = "application/javascript";
                         }
                         else if (context.File.Name.EndsWith("css.gz"))
                         {
                             contentType = "text/css";
                         }
                         headers.Add("Content-Encoding", "gzip");
                         headers["Content-Type"] = contentType;
                     }
                 }
             });*/
            app.UseStaticFiles();

            app.UseKentico();

            app.UseCookiePolicy();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

        }
    }
}
