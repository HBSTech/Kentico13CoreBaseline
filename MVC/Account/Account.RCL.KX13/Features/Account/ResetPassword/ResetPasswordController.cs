using System.Security.Principal;
using XperienceCommunity.Authorization;

namespace Account.Features.Account.ResetPassword
{
    public class ResetPasswordController : Controller
    {
        public const string _routeUrl = "Account/ResetPassword";
        private readonly IUserRepository _userRepository;
        private readonly IAccountSettingsRepository _accountSettingsRepository;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IModelStateService _modelStateService;

        public ResetPasswordController(IUserRepository userRepository,
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
        /// Password Reset, must be authenticated to reset password this way.
        /// </summary>        
        [HttpGet]
        [ControllerActionAuthorization(userAuthenticationRequired: true)]
        [Route(_routeUrl)]
        public ActionResult ResetPassword()
        {
            return View("/Features/Account/ResetPassword/ResetPasswordManual.cshtml");
        }

        /// <summary>
        /// Password Reset, must be authenticated to reset password this way.
        /// </summary>
        [HttpPost]
        [Route(_routeUrl)]
        [ControllerActionAuthorization(userAuthenticationRequired: true)]
        [ExportModelState]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            string resetPasswordUrl = await _accountSettingsRepository.GetAccountResetPasswordUrlAsync(GetUrl());
            var passwordValid = await _userService.ValidatePasswordPolicyAsync(model.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError(nameof(ResetPasswordViewModel.Password), "Password does not meet this site's complexity requirement");
            }
            if (!ModelState.IsValid)
            {
                return Redirect(resetPasswordUrl);
            }
            if (User.Identity.AsMaybe().TryGetValue(out IIdentity identity) && identity.Name.AsNullOrWhitespaceMaybe().TryGetValue(out var name))
            {

                try
                {
                    var currentUser = await _userRepository.GetUserAsync(name);
                    if (currentUser.TryGetValue(out var currentUserVal))
                    {
                        // Everything valid, reset password
                        await _userService.ResetPasswordAsync(currentUserVal, model.Password);
                        model.Succeeded = true;
                    } else
                    {
                        model.Succeeded = false;
                        model.Error = currentUser.Error;
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogException(ex, nameof(ResetPasswordController), "ResetPassword", Description: $"For user {name}");
                    model.Succeeded = false;
                    model.Error = "An error occurred in changing the password.";
                }
            }
            _modelStateService.StoreViewModel(TempData, model);
            return Redirect(resetPasswordUrl);
        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
