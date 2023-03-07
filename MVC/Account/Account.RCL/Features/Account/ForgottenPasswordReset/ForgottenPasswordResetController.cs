using Account.Features.Account.LogIn;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Account.Features.Account.ForgottenPasswordReset
{
    public class ForgottenPasswordResetController : Controller
    {
        public const string _routeUrl = "Account/ForgottenPasswordReset";
        private readonly IUserRepository _userRepository;
        private readonly IAccountSettingsRepository _accountSettingsRepository;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IModelStateService _modelStateService;

        public ForgottenPasswordResetController(IUserRepository userRepository,
            IAccountSettingsRepository accountSettingsRepository,
            IUserService userService,
            ILogger logger,
            IModelStateService modelStateService)
        {
            _userRepository = userRepository;
            _accountSettingsRepository = accountSettingsRepository;
            _userService = userService;
            _logger = logger;
            _modelStateService = modelStateService;
        }

        /// <summary>
        /// Retrieves the UserGUID and the Token and presents the password reset.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(_routeUrl)]
        public ActionResult ForgottenPasswordReset()
        {
            return View("/Features/Account/ForgottenPasswordReset/ForgottenPasswordResetManual.cshtml");
        }

        /// <summary>
        /// Validates and resets the password
        /// </summary>
        [HttpPost]
        [Route(_routeUrl)]
        [ExportModelState]
        public async Task<ActionResult> ForgottenPasswordReset(ForgottenPasswordResetViewModel model)
        {
            var forgottenPasswordResetUrl = await _accountSettingsRepository.GetAccountForgottenPasswordResetUrlAsync(GetUrl());
            var passwordValid = await _userService.ValidatePasswordPolicyAsync(model.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError(nameof(model.Password), "Password does not meet this site's complexity requirement");
            }
            if (!ModelState.IsValid)
            {
                return Redirect(forgottenPasswordResetUrl);
            }

            try
            {
                model.Result = IdentityResult.Failed();
                var userResult = await _userRepository.GetUserAsync(model.UserID);
                if (userResult.IsFailure)
                {
                    model.Result = IdentityResult.Failed(new IdentityError() { Code = "NoUser", Description = userResult.Error });
                }
                else
                {
                    model.Result = await _userService.ResetPasswordFromTokenAsync(userResult.Value, model.Token, model.Password);
                    model.LoginUrl = await _accountSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
                }
            }
            catch (Exception ex)
            {
                model.Result = IdentityResult.Failed(new IdentityError() { Code = "Unknown", Description = "An error occurred." });
                _logger.LogException(ex, nameof(ForgottenPasswordResetController), "ForgottenPasswordReset", Description: $"For userid {model.UserID}");
            }

            // Set this property as the View uses it instead of the IdentityResult, which doesn't serialize/deserialize properly and doesn't make it through the StoreViewModel/GetViewModel
            model.Succeeded = model.Result.Succeeded;
            _modelStateService.StoreViewModel(TempData, model);

            return Redirect(forgottenPasswordResetUrl);
        }

        public static string GetUrl()
        {
            return "/"+ _routeUrl;
        }
    }
}
