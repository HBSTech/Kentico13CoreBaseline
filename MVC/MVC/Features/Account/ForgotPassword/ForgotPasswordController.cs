using Generic.Features.Account.ForgottenPasswordReset;
using Generic.Libraries.Attributes;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Generic.Features.Account.ForgotPassword
{
    public class ForgotPasswordController : Controller
    {
        public const string _routeUrl = "Account/ForgotPassword";
        private readonly IUserRepository _userRepository;
        private readonly ISiteSettingsRepository _siteSettingsRepository;
        private readonly IUserService _userService;
        private readonly IUrlResolver _urlResolver;
        private readonly IModelStateService _modelStateService;

        public ForgotPasswordController(IUserRepository userRepository,
            ISiteSettingsRepository siteSettingsRepository,
            IUserService userService,
            IUrlResolver urlResolver,
            IModelStateService modelStateService)
        {
            _userRepository = userRepository;
            _siteSettingsRepository = siteSettingsRepository;
            _userService = userService;
            _urlResolver = urlResolver;
            _modelStateService = modelStateService;
        }

        /// <summary>
        /// Fallback if not using Page Templates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(_routeUrl)]
        public ActionResult ForgotPassword()
        {
            return View("~/Features/Account/ForgotPassword/ForgotPasswordManual.cshtml");
        }

        /// <summary>
        /// For security, will always show that it sent an email to that user, even if it didn't find it.  The email will contain the link to reset the password
        /// </summary>
        [HttpPost]
        [Route(_routeUrl)]
        [ExportModelState]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            string forgotPasswordUrl = await _siteSettingsRepository.GetAccountForgotPasswordUrlAsync(GetUrl());
            if (!ModelState.IsValid)
            {
                return Redirect(forgotPasswordUrl);
            }

            try
            {
                var user = await _userRepository.GetUserByEmailAsync(model.EmailAddress);
                if (user != null)
                {
                    string forgottenPasswordResetUrl = await _siteSettingsRepository.GetAccountForgottenPasswordResetUrlAsync(ForgottenPasswordResetController.GetUrl());
                    await _userService.SendPasswordResetEmailAsync(user, _urlResolver.GetAbsoluteUrl(forgottenPasswordResetUrl));
                }
                model.Succeeded = true;
            }
            catch (Exception ex)
            {
                model.Succeeded = false;
                model.Error = ex.Message;
            }

            _modelStateService.StoreViewModel(TempData, model);

            return Redirect(forgotPasswordUrl);
        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
