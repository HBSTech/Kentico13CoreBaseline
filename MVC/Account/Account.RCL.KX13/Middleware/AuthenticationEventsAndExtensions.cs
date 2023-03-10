using Account.Features.Account.LogIn;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

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
            if(url.AsNullOrWhitespaceMaybe().TryGetValue(out var accessDeniedUrl)) { 
                context.RedirectUri = accessDeniedUrl;
                await base.RedirectToLogout(context);
            } else {
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
