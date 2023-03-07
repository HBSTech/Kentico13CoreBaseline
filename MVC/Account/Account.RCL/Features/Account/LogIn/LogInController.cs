using Account.Features.Account.MyAccount;
using Kentico.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Account.Features.Account.LogIn
{
    public class LogInController : Controller
    {
        public const string _routeUrl = "Account/LogIn";
        public const string _twoFormAuthenticationUrl = "Account/TwoFormAuthentication";
        private readonly IUserRepository _userRepository;
        private readonly IAccountSettingsRepository _accountSettingsRepository;
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
            IAccountSettingsRepository accountSettingsRepository,
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
            _accountSettingsRepository = accountSettingsRepository;
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
            return View("/Features/Account/LogIn/LogInManual.cshtml");
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
            string loginUrl = await _accountSettingsRepository.GetAccountLoginUrlAsync(GetUrl());

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
                var actualUserResult = await _userRepository.GetUserAsync(model.UserName);
                if (actualUserResult.IsFailure)
                {
                    actualUserResult = await _userRepository.GetUserByEmailAsync(model.UserName);
                }

                if (actualUserResult.IsFailure)
                {
                    ModelState.AddModelError(nameof(model.UserName), actualUserResult.Error);
                    // Store model state, then redirect
                    _modelStateService.StoreViewModel(TempData, model);
                    return Redirect("/" + _twoFormAuthenticationUrl);
                }
                var actualUser = actualUserResult.Value;
                var applicationUser = await _userManager.FindByNameAsync(actualUser.UserName);


                var passwordValid = await _userManager.CheckPasswordAsync(applicationUser, model.Password);

                if (passwordValid && _authenticationConfigurations.UseTwoFormAuthentication())
                {
                    if (await _signInManager.IsTwoFactorClientRememberedAsync(applicationUser))
                    {
                        // Sign in and proceed.
                        await _signInManager.SignInAsync(applicationUser, model.StayLogedIn);
                        return await LoggedInRedirect(model.RedirectUrl);
                    }
                    else
                    {
                        // Send email
                        var token = await _userManager.GenerateTwoFactorTokenAsync(applicationUser, "Email");
                        await _userService.SendVerificationCodeEmailAsync(actualUser, token);

                        // Redirect to Two form auth page
                        var twoFormAuthViewModel = new TwoFormAuthenticationViewModel()
                        {
                            UserName = actualUser.UserName,
                            RedirectUrl = returnUrl,
                            StayLoggedIn = model.StayLogedIn
                        };

                        // Clear login state
                        _modelStateService.ClearViewModel<LogInViewModel>(TempData);

                        // Store model state, then redirect
                        _modelStateService.StoreViewModel(TempData, twoFormAuthViewModel);
                        return Redirect("/" + _twoFormAuthenticationUrl);
                    }
                }

                // Normal sign in
                model.Result = await _signInManager.PasswordSignInAsync(actualUser.UserName, model.Password, model.StayLogedIn, false);
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

            return await LoggedInRedirect(model.RedirectUrl);
        }

        [HttpGet]
        [Route(_twoFormAuthenticationUrl)]
        public async Task<IActionResult> TwoFormAuthentication()
        {
            var model = _modelStateService.GetViewModel<TwoFormAuthenticationViewModel>(TempData).TryGetValue(out var modelVal) ? modelVal : new TwoFormAuthenticationViewModel();

            // Handle no user found
            if (model.UserName.AsNullOrWhitespaceMaybe().TryGetValue(out var userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    string loginUrl = await _accountSettingsRepository.GetAccountLoginUrlAsync(GetUrl());
                    return Redirect(loginUrl);
                }
            }
            return View("/Features/Account/Login/TwoFormAuthentication.cshtml", model);
        }

        [HttpPost]
        [Route(_twoFormAuthenticationUrl)]
        public async Task<IActionResult> TwoFormAuthentication(TwoFormAuthenticationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // This always returns failed, can't figure out why.
                //var result = await  _signInManager.TwoFactorAuthenticatorSignInAsync(model.TwoFormCode, false, false);// model.StayLoggedIn, model.RememberComputer);

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return Redirect($"/{_routeUrl}");
                }

                // Verify token is correct
                var tokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.TwoFormCode);
                if (tokenValid)
                {
                    await _signInManager.SignInAsync(user, model.StayLoggedIn);
                    // Redirectig away from Login, clear TempData so if they return to login it doesn't persist
                    _modelStateService.ClearViewModel<TwoFormAuthenticationViewModel>(TempData);
                    ModelState.Clear();
                    return await LoggedInRedirect(model.RedirectUrl);
                }

                // Invalid token
                model.Failure = true;
                return View("/Features/Account/Login/TwoFormAuthentication.cshtml", model);
            }

            return View("/Features/Account/Login/TwoFormAuthentication.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = "")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.SetParameter("AuthType", "reauthorize");
            return Challenge(properties, provider);
        }

        [HttpGet]
        [Route("Account/ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(LogInViewModel model, string returnUrl = "")
        {
            string loginUrl = await _accountSettingsRepository.GetAccountLoginUrlAsync(GetUrl());

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

                if (updateUser)
                {
                    await _userManager.UpdateAsync(applicationUser);
                }
            }
            else
            {
                // Create new user
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

                await _userService.CreateExternalUserAsync(new User(
                    email: email,
                    userName: email,
                    enabled: true,
                    firstName: firstName,
                    lastName: lastName,
                    isExternal: true,
                    isPublic: false
                    ));
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

            return await LoggedInRedirect(model.RedirectUrl);
        }

        /// <summary>
        /// Logic to redirect upon authentication
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        private async Task<RedirectResult> LoggedInRedirect(string redirectUrl)
        {
            if (await _accountSettingsRepository.GetAccountRedirectToAccountAfterLoginAsync())
            {
                // Redirectig away from Login, clear TempData so if they return to login it doesn't persist
                _modelStateService.ClearViewModel<LogInViewModel>(TempData);
                ModelState.Clear();

                string actualRedirectUrl = "";
                // Try to get returnUrl from query
                if (!string.IsNullOrWhiteSpace(redirectUrl))
                {
                    actualRedirectUrl = redirectUrl;
                }
                if (string.IsNullOrWhiteSpace(redirectUrl))
                {
                    actualRedirectUrl = await _accountSettingsRepository.GetAccountMyAccountUrlAsync("/Account/MyAccount");
                }
                if (string.IsNullOrWhiteSpace(redirectUrl))
                {
                    actualRedirectUrl = "/";
                }
                return Redirect(actualRedirectUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
