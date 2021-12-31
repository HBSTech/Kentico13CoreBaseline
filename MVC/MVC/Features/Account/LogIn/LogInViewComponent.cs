using Generic.Features.Account.ForgotPassword;
using Generic.Features.Account.MyAccount;
using Generic.Features.Account.Registration;
using Generic.Libraries.Attributes;
using Generic.Libraries.Helpers;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Features.Account.LogIn
{
    [ViewComponent]
    public class LogInViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISiteSettingsRepository _siteSettingsRepository;
        private readonly IPageContextRepository _pageContextRepository;

        public LogInViewComponent(IHttpContextAccessor httpContextAccessor,
            ISiteSettingsRepository siteSettingsRepository,
            IPageContextRepository pageContextRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _siteSettingsRepository = siteSettingsRepository;
            _pageContextRepository = pageContextRepository;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(LogInViewModel model = null)
        {
            string redirectUrl = "";
            // Try to get returnUrl from query
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("returnUrl", out StringValues queryReturnUrl) && queryReturnUrl.Any())
            {
                redirectUrl = queryReturnUrl.FirstOrDefault();
            }

            model ??= new LogInViewModel()
            {
                RedirectUrl = redirectUrl,
                MyAccountUrl = await _siteSettingsRepository.GetAccountMyAccountUrlAsync(MyAccountController.GetUrl()),
                RegistrationUrl = await _siteSettingsRepository.GetAccountRegistrationUrlAsync(RegistrationController.GetUrl()),
                ForgotPassword = await _siteSettingsRepository.GetAccountForgotPasswordUrlAsync(ForgotPasswordController.GetUrl())
            };
            model.AlreadyLogedIn = !(await _pageContextRepository.IsEditModeAsync()) && User.Identity.IsAuthenticated;

            return View("~/Features/Account/LogIn/LogIn.cshtml", model);
        }
    }
}
