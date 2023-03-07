using Core.Features.Account.LogIn;
using Generic.Features.HttpErrors;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Generic.App_Start
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
    }

    public class SiteSettingsCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ISiteSettingsRepository _siteSettingsRepository;

        public SiteSettingsCookieAuthenticationEvents(ISiteSettingsRepository siteSettingsRepository)
        {
            _siteSettingsRepository = siteSettingsRepository;
        }
        public override async Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            string url = await _siteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
            string queryString = context.RedirectUri.Contains('?') ? "?" + context.RedirectUri.Split('?')[1] : "";
            context.RedirectUri = url + queryString;
            await base.RedirectToLogout(context);
        }

        public override async Task RedirectToLogout(RedirectContext<CookieAuthenticationOptions> context)
        {
            string url = await _siteSettingsRepository.GetAccountLogOutUrlAsync(LogInController.GetUrl());
            context.RedirectUri = url;
            await base.RedirectToLogout(context);
        }
        public override async Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            string url = await _siteSettingsRepository.GetAccessDeniedUrlAsync(HttpErrorsController.GetAccessDeniedUrl());
            context.RedirectUri = url;
            await base.RedirectToLogout(context);
        }
    }

    public class SiteSettingsFacebookOauthAuthenticationEvents : OAuthEvents
    {
        private readonly ISiteSettingsRepository _siteSettingsRepository;

        public SiteSettingsFacebookOauthAuthenticationEvents(ISiteSettingsRepository siteSettingsRepository)
        {
            _siteSettingsRepository = siteSettingsRepository;
        }

        public override async Task AccessDenied(AccessDeniedContext context)
        {
            context.AccessDeniedPath = await _siteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
            //await base.RedirectToLogout(context);
        }
        public override Task RedirectToAuthorizationEndpoint(RedirectContext<OAuthOptions> context)
        {
            if (context.Properties.Parameters.TryGetValue("AuthType", out object authTypeObj) && authTypeObj is string authType)
            {
                context.RedirectUri = QueryHelpers.AddQueryString(context.RedirectUri, "auth_type", authType);
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    }

    public class SiteSettingsOauthAuthenticationEvents : OAuthEvents
    {
        private readonly ISiteSettingsRepository _siteSettingsRepository;

        public SiteSettingsOauthAuthenticationEvents(ISiteSettingsRepository siteSettingsRepository)
        {
            _siteSettingsRepository = siteSettingsRepository;
        }

        public override async Task AccessDenied(AccessDeniedContext context)
        {
            context.ReturnUrl = await _siteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
            //await base.RedirectToLogout(context);
        }

    }

    public class SiteSettingsTwitterOauthAuthenticationEvents : TwitterEvents
    {
        private readonly ISiteSettingsRepository _siteSettingsRepository;

        public SiteSettingsTwitterOauthAuthenticationEvents(ISiteSettingsRepository siteSettingsRepository)
        {
            _siteSettingsRepository = siteSettingsRepository;
        }

        public override async Task AccessDenied(AccessDeniedContext context)
        {
            context.AccessDeniedPath = await _siteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
            //await base.RedirectToLogout(context);
        }
    }
}
