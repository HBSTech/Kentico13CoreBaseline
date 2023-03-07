using Account.Features.Account.ForgotPassword;
using Account.Features.Account.MyAccount;
using Account.Features.Account.Registration;
using Kentico.Membership;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;

namespace Account.Features.Account.LogIn
{
    [ViewComponent]
    public class LogInViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountSettingsRepository _accountSettingsRepository;
        private readonly IPageContextRepository _pageContextRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IModelStateService _modelStateService;

        public LogInViewComponent(IHttpContextAccessor httpContextAccessor,
            IAccountSettingsRepository accountSettingsRepository,
            IPageContextRepository pageContextRepository,
            SignInManager<ApplicationUser> signInManager,
            IModelStateService modelStateService)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountSettingsRepository = accountSettingsRepository;
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
            if (_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext))
            {
                if (httpContext.Request.Query.TryGetValue("returnUrl", out StringValues queryReturnUrl) && queryReturnUrl.Any())
                {
                    redirectUrl = queryReturnUrl.First();
                }
            }
            // Check google configuration


            var model = new LogInViewModel()
            {
                RedirectUrl = redirectUrl,
                MyAccountUrl = await _accountSettingsRepository.GetAccountMyAccountUrlAsync(MyAccountControllerPath.GetUrl()),
                RegistrationUrl = await _accountSettingsRepository.GetAccountRegistrationUrlAsync(RegistrationController.GetUrl()),
                ForgotPassword = await _accountSettingsRepository.GetAccountForgotPasswordUrlAsync(ForgotPasswordController.GetUrl()),
                ExternalLoginProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };

            // Set this value fresh
            if(User.Identity.AsMaybe().TryGetValue(out var identity))
            {
                model.AlreadyLogedIn = !(await _pageContextRepository.IsEditModeAsync()) && identity.IsAuthenticated;
            } else
            {
                model.AlreadyLogedIn = false;
            }

            return View("/Features/Account/LogIn/LogIn.cshtml", model);
        }
    }
}
