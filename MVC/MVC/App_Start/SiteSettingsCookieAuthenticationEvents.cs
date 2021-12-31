using Generic.Features.Account.LogIn;
using Generic.Features.HttpErrors;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace Generic.App_Start
{
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
            string queryString = context.RedirectUri.Contains('?') ? "?"+context.RedirectUri.Split('?')[1] : "";
            context.RedirectUri = url+ queryString;
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
}