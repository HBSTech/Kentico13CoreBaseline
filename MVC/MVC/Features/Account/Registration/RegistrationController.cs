using Generic.Features.Account.Confirmation;
using Generic.Libraries.Attributes;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Text.Json;
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
        private readonly IModelStateService _modelStateService;

        public RegistrationController(ISiteSettingsRepository siteSettingsRepository,
            IUserService userService,
            ILogger logger,
            IUrlResolver urlResolver,
            IModelStateService modelStateService
            )
        {
            _siteSettingsRepository = siteSettingsRepository;
            _userService = userService;
            _logger = logger;
            _urlResolver = urlResolver;
            _modelStateService = modelStateService;
        }

        

        /// <summary>
        /// Fall back, should use Account Page Templates instead
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(_routeUrl)]
        public ActionResult Registration()
        {
            return View("~/Features/Account/Registration/RegistrationManual.cshtml");
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
            var registrationUrl = await _siteSettingsRepository.GetAccountRegistrationUrlAsync(GetUrl());
            // Ensure valid
            var passwordValid = await _userService.ValidatePasswordPolicyAsync(userAccountModel.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError(nameof(RegistrationViewModel.Password), "Password does not meet this site's complexity requirement");
            }
            if (!ModelState.IsValid || !passwordValid)
            {
                return Redirect(registrationUrl);
            }

            // Create a basic Kentico User and assign the portal ID
            try
            {
                var newUser = await _userService.CreateUserAsync(userAccountModel.User.GetUser(), userAccountModel.Password);

                // Send confirmation email with registration link
                string confirmationUrl = await _siteSettingsRepository.GetAccountConfirmationUrlAsync(ConfirmationController.GetUrl());
                await _userService.SendRegistrationConfirmationEmailAsync(newUser, _urlResolver.GetAbsoluteUrl(confirmationUrl));
                userAccountModel.RegistrationSuccessful = true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, nameof(RegistrationController), "Registration", Description: $"For User {userAccountModel.User}");
                userAccountModel.RegistrationFailureMessage = ex.Message;
                userAccountModel.RegistrationSuccessful = false;
            }

            // Store view model for retrieval
            _modelStateService.StoreViewModel<RegistrationViewModel>(TempData, userAccountModel);

            return Redirect(registrationUrl);

        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
