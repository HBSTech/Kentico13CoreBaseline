using Generic.Features.Account.MyAccount;
using Generic.Libraries.Attributes;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Kentico.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Generic.Features.Account.LogIn
{
    public class LogInController : Controller
    {
        public const string _routeUrl = "Account/LogIn";
        private readonly IUserRepository _userRepository;
        private readonly ISiteSettingsRepository _siteSettingsRepository;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogInController(IUserRepository userRepository,
            ISiteSettingsRepository siteSettingsRepository,
            IUserService userService,
            ILogger logger,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _siteSettingsRepository = siteSettingsRepository;
            _userService = userService;
            _logger = logger;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Fall back if not using Page Templates
        /// </summary>
        [HttpGet]
        [Route(_routeUrl)]
        public ActionResult LogIn()
        {
            return View("~/Features/Account/LogIn/LogInManual.cshtml");
        }

        /// <summary>
        /// Handles authentication when the sign-in form is submitted. Accepts parameters posted from the sign-in form via the LogInViewModel.
        /// </summary>
        [HttpPost]
        [Route(_routeUrl)]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<ActionResult> LogIn(LogInViewModel model, [FromQuery] string returnUrl)
        {
            string loginUrl = await _siteSettingsRepository.GetAccountLoginUrlAsync(GetUrl());

            // Validates the received user credentials based on the view model
            if (!ModelState.IsValid)
            {
                // Displays the sign-in form if the user credentials are invalid
                return Redirect(loginUrl);
            }

            // Attempts to authenticate the user against the Kentico database
            model.Result = SignInResult.Failed;
            try
            {
                var actualUser = await _userRepository.GetUserAsync(model.UserName);
                actualUser ??= await _userRepository.GetUserByEmailAsync(model.UserName);
                model.Result = await _signInManager.PasswordSignInAsync(actualUser?.UserName, model.Password, model.StayLogedIn, false);
            }
            catch (Exception ex)
            {
                // Logs an error into the Kentico event log if the authentication fails
                _logger.LogException(ex, nameof(LogInController), "Login", Description: $"For user {model.UserName}");
            }

            // If the authentication was not successful, displays the sign-in form with an "Authentication failed" message
            if (model.Result != SignInResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Authentication failed");
                return Redirect(loginUrl);
            }

            if (await _siteSettingsRepository.GetAccountRedirectToAccountAfterLoginAsync())
            {
                string redirectUrl = "";
                // Try to get returnUrl from query
                if (!string.IsNullOrWhiteSpace(model.RedirectUrl))
                {
                    redirectUrl = model.RedirectUrl;
                }
                if (string.IsNullOrWhiteSpace(redirectUrl))
                {
                    redirectUrl = await _siteSettingsRepository.GetAccountMyAccountUrlAsync(MyAccountController.GetUrl());
                }
                if (string.IsNullOrWhiteSpace(redirectUrl))
                {
                    redirectUrl = "/";
                }
                return Redirect(redirectUrl);
            }
            else
            {
                return Redirect(loginUrl);
            }
        }
        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
