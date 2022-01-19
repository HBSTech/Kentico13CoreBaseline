using Generic.Libraries.Attributes;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;
using XperienceCommunity.Authorization;

namespace Generic.Features.Account.ResetPassword
{
    public class ResetPasswordController : Controller
    {
        public const string _routeUrl = "Account/ResetPassword";
        private readonly IUserRepository _userRepository;
        private readonly ISiteSettingsRepository _siteSettingsRepository;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IModelStateService _modelStateService;

        public ResetPasswordController(IUserRepository userRepository,
            ISiteSettingsRepository siteSettingsRepository,
            IUserService userService,
            ILogger logger,
            IModelStateService modelStateService)
        {
            _userRepository = userRepository;
            _siteSettingsRepository = siteSettingsRepository;
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
            return View("~/Features/Account/ResetPassword/ResetPasswordManual.cshtml");
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
            string resetPasswordUrl = await _siteSettingsRepository.GetAccountResetPasswordUrlAsync(GetUrl());
            var passwordValid = await _userService.ValidatePasswordPolicyAsync(model.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError(nameof(ResetPasswordViewModel.Password), "Password does not meet this site's complexity requirement");
            }
            if (!ModelState.IsValid)
            {
                return Redirect(resetPasswordUrl);
            }
            try
            {
                var currentUser = await _userRepository.GetUserAsync(User.Identity.Name);

                // Everything valid, reset password
                await _userService.ResetPasswordAsync(currentUser, model.Password);
                model.Succeeded = true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, nameof(ResetPasswordController), "ResetPassword", Description: $"For user {User.Identity.Name}");
                model.Succeeded = false;
                model.Error = "An error occurred in changing the password.";
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
