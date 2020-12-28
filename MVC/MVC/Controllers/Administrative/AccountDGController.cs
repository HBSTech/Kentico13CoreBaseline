using System;
using System.Net;
using System.Threading.Tasks;
using CMS.Activities.Loggers;
using CMS.Base;
using CMS.Base.UploadExtensions;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using DancingGoat.Models;
using Kentico.Membership;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

/// <summary>
/// This is the Dancing Goat implementation of the Account Controller, if you wish to use it, you can!
/// </summary>
namespace Generic.Controllers
{
    public class AccountDGController : Controller
    {
        private readonly IMembershipActivityLogger membershipActivitiesLogger;
        private readonly IStringLocalizer<SharedResources> localizer;
        private readonly IEventLogService eventLogService;
        private readonly IAvatarService avatarService;
        private readonly ApplicationUserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ISiteService siteService;
        private readonly IMessageService emailService;


        public AccountDGController(ApplicationUserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IMessageService emailService,
                                 ISiteService siteService,
                                 IAvatarService avatarService,
                                 IMembershipActivityLogger membershipActivitiesLogger,
                                 IStringLocalizer<SharedResources> localizer,
                                 IEventLogService eventLogService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.siteService = siteService;
            this.avatarService = avatarService;
            this.membershipActivitiesLogger = membershipActivitiesLogger;
            this.localizer = localizer;
            this.eventLogService = eventLogService;
        }


        // GET: Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var signInResult = SignInResult.Failed;

            try
            {
                signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.StaySignedIn, false);
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AccountController", "Login", ex);
            }

            if (signInResult.Succeeded)
            {
                ContactManagementContext.UpdateUserLoginContact(model.UserName);

                membershipActivitiesLogger.LogLogin(model.UserName);

                var decodedReturnUrl = WebUtility.UrlDecode(returnUrl);
                if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
                {
                    return View("RedirectAfterLogin", decodedReturnUrl);
                }

                return Redirect("/");
            }

            if (signInResult.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, localizer["Your account requires activation before logging in."]);
            }
            else
            {
                ModelState.AddModelError(string.Empty, localizer["Your sign-in attempt was not successful. Please try again."].ToString());
            }

            return View(model);
        }


        // POST: Account/Logout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            signInManager.SignOutAsync();
            return Redirect("/");
        }


        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }


        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.UserName,
                FullName = UserInfoProvider.GetFullName(model.FirstName, null, model.LastName),
                Enabled = true
            };

            var registerResult = new IdentityResult();

            try
            {
                registerResult = await userManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AccountController", "Register", ex);
                ModelState.AddModelError(string.Empty, localizer["Your registration was not successful."]);
            }

            if (registerResult.Succeeded)
            {
                membershipActivitiesLogger.LogRegistration(model.UserName);

                var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, false);

                if (signInResult.Succeeded)
                {
                    ContactManagementContext.UpdateUserLoginContact(model.UserName);

                    SendRegistrationSuccessfulEmail(user.Email);
                    membershipActivitiesLogger.LogLogin(model.UserName);

                    return Redirect("/");
                }

                if (signInResult.IsNotAllowed)
                {
                    if (user.WaitingForApproval)
                    {
                        SendWaitForApprovalEmail(user.Email);
                    }

                    return RedirectToAction(nameof(RequireConfirmedAccount));
                }
            }

            foreach (var error in registerResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        // GET: Account/RetrievePassword
        public ActionResult RetrievePassword()
        {
            return PartialView("_RetrievePassword");
        }


        // POST: Account/RetrievePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RetrievePassword(RetrievePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_RetrievePassword", model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var url = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, token }, RequestContext.URL.Scheme);

                await emailService.SendEmailAsync(user.Email, localizer["Request for changing your password"],
                    string.Format(localizer["You have submitted a request to change your password. " +
                    "Please click <a href=\"{0}\">this link</a> to set a new password.<br/><br/> " +
                    "If you did not submit the request please let us know."], url));
            }

            return Content(localizer["If the email address is known to us, we'll send a password recovery link in a few minutes."]);
        }


        // GET: Account/ResetPassword
        [HttpGet]
        public async Task<ActionResult> ResetPassword(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel()
            {
                UserId = user.Id,
                Token = token
            };

            return View(model);
        }


        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.UserId.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var resetResult = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (resetResult.Succeeded)
            {
                return View("ResetPasswordSucceeded");
            }

            foreach (var error in resetResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        // GET: Account/YourAccount
        [Authorize]
        public async Task<ActionResult> YourAccount(bool avatarUpdateFailed = false)
        {
            var model = new YourAccountViewModel
            {
                User = await userManager.FindByNameAsync(User.Identity.Name),
                AvatarUpdateFailed = avatarUpdateFailed
            };

            return View(model);
        }


        // GET: Account/Edit
        [Authorize]
        public async Task<ActionResult> Edit()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var model = new PersonalDetailsViewModel(user);
            return View(model);
        }


        // POST: Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PersonalDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UserName = User.Identity.Name;
                return View(model);
            }

            try
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);

                // Set full name only if it was automatically generated
                if (user.FullName == UserInfoProvider.GetFullName(user.FirstName, null, user.LastName))
                {
                    user.FullName = UserInfoProvider.GetFullName(model.FirstName, null, model.LastName);
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                await userManager.UpdateAsync(user);

                return RedirectToAction(nameof(YourAccount));
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AccountController", "Edit", ex);
                ModelState.AddModelError(string.Empty, localizer["Personal details save failed"]);

                model.UserName = User.Identity.Name;
                return View(model);
            }
        }


        // POST: Account/ChangeAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> ChangeAvatar(IFormFile avatarUpload)
        {
            object routeValues = null;

            if (avatarUpload != null && avatarUpload.Length > 0)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                if (!avatarService.UpdateAvatar(avatarUpload.ToUploadedFile(), user.Id, siteService.CurrentSite.SiteName))
                {
                    routeValues = new { avatarUpdateFailed = true };
                }
            }

            return RedirectToAction(nameof(YourAccount), routeValues);
        }


        // GET: Account/RequireConfirmedAccount
        [HttpGet]
        public ActionResult RequireConfirmedAccount()
        {
            return View();
        }


        private void SendRegistrationSuccessfulEmail(string email)
        {
            var subject = localizer["Registration information"];
            var body = localizer["Thank you for registering at our site."];

            emailService.SendEmailAsync(email, subject, body);
        }


        private void SendWaitForApprovalEmail(string email)
        {
            var subject = localizer["Your registration must be approved"];
            var body = string.Format(localizer["Thank you for registering at our site {0}. Your registration must be approved by administrator."], siteService.CurrentSite.DisplayName);

            emailService.SendEmailAsync(email, subject, body);
        }
    }
}