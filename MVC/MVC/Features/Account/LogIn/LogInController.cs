﻿using Generic.Features.Account.MyAccount;
using Generic.Libraries.Attributes;
using Generic.Models.Account;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Kentico.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
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
        private readonly IRoleService _roleService;
        private readonly ISiteRepository _siteRepository;
        private readonly IModelStateService _modelStateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationConfigurations _authenticationConfigurations;

        public LogInController(IUserRepository userRepository,
            ISiteSettingsRepository siteSettingsRepository,
            IUserService userService,
            ILogger logger,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IRoleService roleService,
            ISiteRepository siteRepository,
            IModelStateService modelStateService,
            UserManager<ApplicationUser> userManager,
            IAuthenticationConfigurations authenticationConfigurations)
        {
            _userRepository = userRepository;
            _siteSettingsRepository = siteSettingsRepository;
            _userService = userService;
            _logger = logger;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _siteRepository = siteRepository;
            _modelStateService = modelStateService;
            _userManager = userManager;
            _authenticationConfigurations = authenticationConfigurations;
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

            // Store results
            _modelStateService.StoreViewModel(TempData, model);

            // If the authentication was not successful, displays the sign-in form with an "Authentication failed" message
            if (model.Result != SignInResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Authentication failed");
                return Redirect(loginUrl);
            }

            if (await _siteSettingsRepository.GetAccountRedirectToAccountAfterLoginAsync())
            {
                // Redirectig away from Login, clear TempData so if they return to login it doesn't persist
                _modelStateService.ClearViewModel<LogInViewModel>(TempData);
                ModelState.Clear();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.SetParameter("AuthType", "reauthorize");
            return Challenge(properties, provider);
        }

        [HttpGet]
        [Route("Account/ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(LogInViewModel model, string returnUrl = null)
        {
            string loginUrl = await _siteSettingsRepository.GetAccountLoginUrlAsync(GetUrl());

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return Redirect(loginUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                return Redirect(loginUrl);
            }

            var applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser != null)
            {
                bool updateUser = false;
                // If user started account through email, but then didn't confirm and 
                // then authenticated 3rd party, their email is now 'confirmed' so will enable
                // and set that to true.  This way you can STILL disable an existing account
                if (!applicationUser.Enabled && !applicationUser.EmailConfirmed)
                {
                    applicationUser.Enabled = true;
                    applicationUser.EmailConfirmed = true;
                    updateUser = true;
                }

                // Depending on setting, convert to external only
                if (!applicationUser.IsExternal && _authenticationConfigurations.GetExistingInternalUserBehavior() != ExistingInternalUserBehavior.LeaveAsIs)
                {
                    applicationUser.IsExternal = true;
                    updateUser = true;
                }

                if(updateUser)
                {
                    await _userManager.UpdateAsync(applicationUser);
                }
            }
            else
            {
                // Create new user
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

                await _userService.CreateExternalUserAsync(new Models.User()
                {
                    Email = email,
                    UserName = email,
                    Enabled = true,
                    FirstName = firstName,
                    LastName = lastName,
                    IsExternal = true
                });
                applicationUser = await _userManager.FindByEmailAsync(email);
            }

            // Sign in
            await _signInManager.SignInAsync(applicationUser, model.StayLogedIn);

            var externalUserRoles = _authenticationConfigurations.AllExternalUserRoles().ToList();
            
            switch (info.LoginProvider.ToLowerInvariant())
            {
                case "microsoft":
                    externalUserRoles.AddRange(_authenticationConfigurations.MicrosoftUserRoles());
                    break;
                case "twitter":
                    externalUserRoles.AddRange(_authenticationConfigurations.TwitterUserRoles());
                    break;
                case "facebook":
                    externalUserRoles.AddRange(_authenticationConfigurations.FacebookUserRoles());
                    break;
                case "google":
                    externalUserRoles.AddRange(_authenticationConfigurations.GoogleUserRoles());
                    break;
                default:
                    break;
            }

            foreach (string role in externalUserRoles)
            {
                await _roleService.CreateRoleIfNotExisting(role, _siteRepository.CurrentSiteName());
                await _roleService.SetUserRole(applicationUser.Id, role, _siteRepository.CurrentSiteName(), true);
            }
            model.Result = SignInResult.Success;

            if (await _siteSettingsRepository.GetAccountRedirectToAccountAfterLoginAsync())
            {
                // Clear ModelState and TempData 

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
                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }
        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
