using Generic.Features.Account.MyAccount;
using Generic.Libraries.Attributes;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Kentico.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

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
        private readonly IModelStateService _modelStateService;
        private readonly IRoleService _roleService;
        private readonly ISiteRepository _siteRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public LogInController(IUserRepository userRepository,
            ISiteSettingsRepository siteSettingsRepository,
            IUserService userService,
            ILogger logger,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IModelStateService modelStateService,
            IRoleService roleService,
            ISiteRepository siteRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _siteSettingsRepository = siteSettingsRepository;
            _userService = userService;
            _logger = logger;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _modelStateService = modelStateService;
            _roleService = roleService;
            _siteRepository = siteRepository;
            _userManager = userManager;
            _configuration = configuration;
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

        [Route("Account/GoogleResponse")]
        public async Task<ActionResult> GoogleResponse(LogInViewModel model)
        {
            string loginUrl = await _siteSettingsRepository.GetAccountLoginUrlAsync(GetUrl());

            var googleAuth = _configuration.GetSection("Authentication:Google");
            string clientID = googleAuth["ClientId"];

            var googlecredentials = _httpContextAccessor.HttpContext.Request.Form;

            var cred = googlecredentials["credential"];

            var g_csrf_token = googlecredentials["g_csrf_token"];
            var g_csrf_token_cookie = string.Empty;
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("g_csrf_token", out g_csrf_token_cookie);

            if (!g_csrf_token_cookie.Equals(g_csrf_token, StringComparison.InvariantCultureIgnoreCase))
            {
                model.Result = SignInResult.Failed;
                ModelState.AddModelError(string.Empty, "Authentication failed");

                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }

            Payload ValidPayload = null;
            try
            {
                ValidPayload = await GoogleJsonWebSignature.ValidateAsync(cred);
            }
            catch (Exception ex)
            {
                model.Result = SignInResult.Failed;
                ModelState.AddModelError(string.Empty, "Authentication failed");

                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }
            if (!((string)ValidPayload.Audience).Equals(clientID, StringComparer.InvariantCultureIgnoreCase))
            {
                model.Result = SignInResult.Failed;
                ModelState.AddModelError(string.Empty, "Authentication failed");

                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }
            if (ValidPayload.Issuer != "accounts.google.com" && ValidPayload.Issuer != "https://accounts.google.com")
            {
                model.Result = SignInResult.Failed;
                ModelState.AddModelError(string.Empty, "Authentication failed");

                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }
            if (ValidPayload.ExpirationTimeSeconds < DateTimeOffset.Now.ToUnixTimeSeconds())
            {
                model.Result = SignInResult.Failed;
                ModelState.AddModelError(string.Empty, "Authentication failed");

                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }





            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(cred);
            var email = jwtSecurityToken.Claims.First(claim => claim.Type == "email")?.Value ?? String.Empty;
            var firstName = jwtSecurityToken.Claims.First(claim => claim.Type == "given_name")?.Value ?? String.Empty;
            var lastName = jwtSecurityToken.Claims.First(claim => claim.Type == "family_name")?.Value ?? String.Empty;
            var verifiedEmail = Convert.ToBoolean(jwtSecurityToken.Claims.First(claim => claim.Type == "email_verified")?.Value ?? "false");

            /*
            var email = ValidPayload.Email?.Value ?? String.Empty;
            var firstName = ValidPayload.GivenName?.Value ?? String.Empty;
            var lastName = ValidPayload.FamilyName?.Value ?? String.Empty;
            var verifiedEmail = Convert.ToBoolean(ValidPayload.EmailVerified)?.Value ?? "false");
            */

            var actualUser = await _userRepository.GetUserByEmailAsync(email);
            if (actualUser != null)
            {
                if (!actualUser.IsExternal)
                {
                    // GET OPTION on what to do when user exists
                }


            } else
            {
                // Create new user
                await _userService.CreateExternalUserAsync(new Models.User()
                {
                    Email = email,
                    UserName = email,
                    Enabled = true,
                    FirstName = firstName,
                    LastName = lastName,
                    IsExternal = true
                });
            }

            // Retrieve user now by email
            var foundAppUser = await _userManager.FindByEmailAsync(email);
            // Sign in
            await _signInManager.SignInAsync(foundAppUser, model.StayLogedIn);

            var defaultRoles = Array.Empty<string>(); // Replace with settings
            foreach (string role in defaultRoles)
            {
                await _roleService.SetUserRole(foundAppUser.Id, role, _siteRepository.CurrentSiteName(), true);
            }
            model.Result = SignInResult.Success;

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
                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
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

            var actualUser = await _userRepository.GetUserByEmailAsync(email);
            if (actualUser != null)
            {
                if (!actualUser.IsExternal)
                {
                    if (!actualUser.Enabled)
                    {
                        return RedirectToAction("Index", "Home");
                    }
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
            }

            // Retrieve user now by email
            var foundAppUser = await _userManager.FindByEmailAsync(email);
            // Sign in
            await _signInManager.SignInAsync(foundAppUser, model.StayLogedIn);

            var defaultRoles = Array.Empty<string>(); // Replace with settings
            foreach (string role in defaultRoles)
            {
                await _roleService.SetUserRole(foundAppUser.Id, role, _siteRepository.CurrentSiteName(), true);
            }
            model.Result = SignInResult.Success;

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
                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect(loginUrl);
            }
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginModel model, string returnUrl = null)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                //TODO Enter Error View
                return View("Error");
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            IdentityResult result;

            if (user != null)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }
            else
            {
                model.Principal = info.Principal;
                // Create new user
                await _userService.CreateExternalUserAsync(new Models.User()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Enabled = true,
                    //FirstName = firstName,
                    //LastName = lastName,
                    IsExternal = true
                });
                
            }
            // Retrieve user now by email
            var foundAppUser = await _userManager.FindByEmailAsync(model.Email);
            // Sign in
            await _signInManager.SignInAsync(foundAppUser, false);

            var defaultRoles = Array.Empty<string>(); // Replace with settings
            foreach (string role in defaultRoles)
            {
                await _roleService.SetUserRole(foundAppUser.Id, role, _siteRepository.CurrentSiteName(), true);
            }
            model.Result = SignInResult.Success;

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
                // Store the ViewModel in tempdata _modelStateService.StoreViewModel(TempData, model);
                return Redirect("Account/Login2");
            }
        }*/

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
    public class ExternalLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public bool StayLogedIn { get; set; }
        public string MyAccountUrl { get; set; }
        public string RegistrationUrl { get; set; }
        public string ForgotPassword { get; set; }
        public bool AlreadyLogedIn { get; set; } = false;
        public SignInResult Result { get; set; }
        public string RedirectUrl { get; set; }
    }
}
