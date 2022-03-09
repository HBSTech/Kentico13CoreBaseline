using Generic.Features.Account.ForgotPassword;
using Generic.Features.Account.MyAccount;
using Generic.Features.Account.Registration;
using Generic.Libraries.Attributes;
using Generic.Libraries.Helpers;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Kentico.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IModelStateService _modelStateService;

        public LogInViewComponent(IHttpContextAccessor httpContextAccessor,
            ISiteSettingsRepository siteSettingsRepository,
            IPageContextRepository pageContextRepository,
            SignInManager<ApplicationUser> signInManager,
            IModelStateService modelStateService)
        {
            _httpContextAccessor = httpContextAccessor;
            _siteSettingsRepository = siteSettingsRepository;
            _pageContextRepository = pageContextRepository;
            _signInManager = signInManager;
            _modelStateService = modelStateService;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Merge Model State
            _modelStateService.MergeModelState(ModelState, TempData);

            string redirectUrl = "";

            // Try to get returnUrl from query
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("returnUrl", out StringValues queryReturnUrl) && queryReturnUrl.Any())
            {
                redirectUrl = queryReturnUrl.FirstOrDefault();
            }

            // Check google configuration
            

            var model = new LogInViewModel()
            {
                RedirectUrl = redirectUrl,
                MyAccountUrl = await _siteSettingsRepository.GetAccountMyAccountUrlAsync(MyAccountController.GetUrl()),
                RegistrationUrl = await _siteSettingsRepository.GetAccountRegistrationUrlAsync(RegistrationController.GetUrl()),
                ForgotPassword = await _siteSettingsRepository.GetAccountForgotPasswordUrlAsync(ForgotPasswordController.GetUrl()),
                ExternalLoginProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };

            // Set this value fresh
            model.AlreadyLogedIn = !(await _pageContextRepository.IsEditModeAsync()) && User.Identity.IsAuthenticated;

            return View("~/Features/Account/LogIn/LogIn.cshtml", model);
        }
    }
}
