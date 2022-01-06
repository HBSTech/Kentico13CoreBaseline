using Generic.Features.Account.Confirmation;
using Generic.Libraries.Attributes;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Generic.Features.Account.Registration
{
    public class RegistrationController : Controller
    {
        public const string _routeUrl = "Account/Registration";
        private readonly ISiteSettingsRepository _siteSettingsRepository;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IUrlResolver _urlResolver;

        public RegistrationController(ISiteSettingsRepository siteSettingsRepository,
            IUserService userService,
            ILogger logger,
            IUrlResolver urlResolver)
        {
            _siteSettingsRepository = siteSettingsRepository;
            _userService = userService;
            _logger = logger;
            _urlResolver = urlResolver;
        }

        

        /// <summary>
        /// Fall back, should use Account Page Templates instead
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(_routeUrl)]
        public ActionResult Registration()
        {
            return View("~/Features/Account/Register/RegisterManual.cshtml");
        }

        /// <summary>
        /// Registers the User, uses Email confirmation
        /// </summary>
        /// <param name="UserAccountModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(_routeUrl)]
        [ExportModelState]
        public async Task<ActionResult> Registration(RegistrationViewModel userAccountModel)
        {
            var registerUrl = await _siteSettingsRepository.GetAccountRegistrationUrlAsync(GetUrl());
            // Ensure valid
            var passwordValid = await _userService.ValidatePasswordPolicyAsync(userAccountModel.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError(nameof(RegistrationViewModel.Password), "Password does not meet this site's complexity requirement");
            }
            if (!ModelState.IsValid || !passwordValid)
            {
                return Redirect(registerUrl);
            }

            // Create a basic Kentico User and assign the portal ID
            try
            {
                var newUser = await _userService.CreateUserAsync(userAccountModel.User.GetUser(), userAccountModel.Password);

                // Send confirmation email with registration link
                string confirmationUrl = await _siteSettingsRepository.GetAccountConfirmationUrlAsync(ConfirmationController.GetUrl());
                await _userService.SendRegistrationConfirmationEmailAsync(newUser, _urlResolver.GetAbsoluteUrl(confirmationUrl));
                userAccountModel.RegisterationSuccessful = true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, nameof(RegistrationController), "Registration", Description: $"For User {userAccountModel.User}");
                userAccountModel.RegistrationFailureMessage = ex.Message;
                userAccountModel.RegisterationSuccessful = false;
            }

            return Redirect(registerUrl);

        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
