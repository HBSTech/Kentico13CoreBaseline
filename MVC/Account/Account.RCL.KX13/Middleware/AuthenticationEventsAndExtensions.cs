using Account.Features.Account.LogIn;
using Account.Repositories.Implementation;
using CMS.Helpers;
using Kentico.Membership;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using XperienceCommunity.Authorization;

namespace Account
{
    public static class AuthenticationServiceExtensions

    {
        /// <summary>
        /// Registers Authentication Events and related interfaces
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
        {
            services.AddScoped<SiteSettingsCookieAuthenticationEvents>();
            services.AddScoped<SiteSettingsOauthAuthenticationEvents>();
            services.AddScoped<SiteSettingsFacebookOauthAuthenticationEvents>();
            services.AddScoped<SiteSettingsTwitterOauthAuthenticationEvents>();
            return services;
        }

        public static IMvcBuilder AddControllersWithViewsWithKenticoAuthorization(this IServiceCollection services)
        {
            return services.AddControllersWithViews(opt => opt.Filters.AddKenticoAuthorization());
        }

        public static IServiceCollection AddKenticoAuthentication(this IServiceCollection services, IConfiguration configuration, string AUTHENTICATION_COOKIE_NAME = "identity.authentication")
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
            var authBuilder = services.AddAuthentication();

            var googleAuth = configuration.GetSection("Authentication:Google");
            if (googleAuth.Exists())
            {
                authBuilder.AddGoogle("Google", opt =>
                {
                    opt.ClientId = googleAuth["ClientId"];
                    opt.ClientSecret = googleAuth["ClientSecret"];
                    opt.SignInScheme = IdentityConstants.ExternalScheme;
                    opt.EventsType = typeof(SiteSettingsOauthAuthenticationEvents);
                });
            }
            var facebookAuth = configuration.GetSection("Authentication:Facebook");
            if (facebookAuth.Exists())
            {
                authBuilder.AddFacebook("Facebook", opt =>
                 {
                     opt.AppId = facebookAuth["AppId"];
                     opt.AppSecret = facebookAuth["AppSecret"];
                     opt.SignInScheme = IdentityConstants.ExternalScheme;
                     opt.EventsType = typeof(SiteSettingsFacebookOauthAuthenticationEvents);
                 });
            }
            var twitterAuth = configuration.GetSection("Authentication:Twitter");
            if(twitterAuth.Exists())
            {
                authBuilder.AddTwitter(opt =>
                {
                    opt.ConsumerKey = twitterAuth["APIKey"];
                    opt.ConsumerSecret = twitterAuth["APIKeySecret"];
                    opt.RetrieveUserDetails = true;
                    opt.EventsType = typeof(SiteSettingsTwitterOauthAuthenticationEvents);
                });
            }
            var microsoftAuth = configuration.GetSection("Authentication:Microsoft");
            if (microsoftAuth.Exists())
            {
                authBuilder.AddMicrosoftAccount(opt =>
                 {

                     opt.ClientId = microsoftAuth["ClientId"];
                     opt.ClientSecret = microsoftAuth["ClientSecret"];
                     opt.EventsType = typeof(SiteSettingsOauthAuthenticationEvents);
                 });
            }
            
            // Baseline Configuration of External Authentication
            authBuilder.ConfigureAuthentication(config =>
                {
                    config.ExistingInternalUserBehavior = ExistingInternalUserBehavior.SetToExternal;
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
            
            return services;
        }
    }

    public class SiteSettingsCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IAccountSettingsRepository _accountSiteSettingsRepository;

        public SiteSettingsCookieAuthenticationEvents(IAccountSettingsRepository accountSiteSettingsRepository)
        {
            _accountSiteSettingsRepository = accountSiteSettingsRepository;
        }
        public override async Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            string url = await _accountSiteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
            string queryString = context.RedirectUri.Contains('?') ? "?" + context.RedirectUri.Split('?')[1] : "";
            context.RedirectUri = url + queryString;
            await base.RedirectToLogin(context);
        }

        public override async Task RedirectToLogout(RedirectContext<CookieAuthenticationOptions> context)
        {
            string url = await _accountSiteSettingsRepository.GetAccountLogOutUrlAsync(LogInController.GetUrl());
            context.RedirectUri = url;
            await base.RedirectToLogout(context);
        }
        public override async Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            string url = await _accountSiteSettingsRepository.GetAccessDeniedUrlAsync(string.Empty);
            if (url.AsNullOrWhitespaceMaybe().TryGetValue(out var accessDeniedUrl))
            {
                context.RedirectUri = accessDeniedUrl;
                await base.RedirectToLogout(context);
            }
            else
            {
                await base.RedirectToLogout(context);
            }
        }
    }

    public class SiteSettingsFacebookOauthAuthenticationEvents : OAuthEvents
    {
        private readonly IAccountSettingsRepository _accountSiteSettingsRepository;

        public SiteSettingsFacebookOauthAuthenticationEvents(IAccountSettingsRepository accountSiteSettingsRepository)
        {
            _accountSiteSettingsRepository = accountSiteSettingsRepository;
        }

        public override async Task AccessDenied(AccessDeniedContext context)
        {
            context.AccessDeniedPath = await _accountSiteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
        }
        public override Task RedirectToAuthorizationEndpoint(RedirectContext<OAuthOptions> context)
        {
            if (context.Properties.Parameters.TryGetValue("AuthType", out var authTypeObj) && authTypeObj is string authType)
            {
                context.RedirectUri = QueryHelpers.AddQueryString(context.RedirectUri, "auth_type", authType);
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    }

    public class SiteSettingsOauthAuthenticationEvents : OAuthEvents
    {
        private readonly IAccountSettingsRepository _accountSiteSettingsRepository;

        public SiteSettingsOauthAuthenticationEvents(IAccountSettingsRepository accountSiteSettingsRepository)
        {
            _accountSiteSettingsRepository = accountSiteSettingsRepository;
        }

        public override async Task AccessDenied(AccessDeniedContext context)
        {
            context.ReturnUrl = await _accountSiteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
            //await base.RedirectToLogout(context);
        }

    }

    public class SiteSettingsTwitterOauthAuthenticationEvents : TwitterEvents
    {
        private readonly IAccountSettingsRepository _accountSiteSettingsRepository;

        public SiteSettingsTwitterOauthAuthenticationEvents(IAccountSettingsRepository accountSiteSettingsRepository)
        {
            _accountSiteSettingsRepository = accountSiteSettingsRepository;
        }

        public override async Task AccessDenied(AccessDeniedContext context)
        {
            context.AccessDeniedPath = await _accountSiteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
            //await base.RedirectToLogout(context);
        }
    }
}
