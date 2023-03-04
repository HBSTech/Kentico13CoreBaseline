using Account.Services;
using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using Kentico.Membership;
using Microsoft.AspNetCore.Identity;
using MVCCaching;
using System.Web;

namespace Account.Services.Implementation
{
    [AutoDependencyInjection]
    public class UserService : IUserService
    {
        private readonly ApplicationUserManager<ApplicationUser> _UserManager;
        private readonly IUserInfoProvider _UserInfoProvider;
        private readonly IMessageService _EmailService;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ISiteRepository _siteRepository;


        public UserService(
            IUserInfoProvider UserInfoProvider,
            ApplicationUserManager<ApplicationUser> userManager,
            IMessageService emailService,
            IProgressiveCache progressiveCache,
            ISiteRepository siteRepository)
        {
            _UserManager = userManager;
            _UserInfoProvider = UserInfoProvider;
            _EmailService = emailService;
            _progressiveCache = progressiveCache;
            _siteRepository = siteRepository;
        }

        public Task<User> CreateUserAsync(User user, string password, bool enabled = false)
        {
            // Create basic user
            var newUser = new UserInfo()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                SiteIndependentPrivilegeLevel = UserPrivilegeLevelEnum.None,
                Enabled = enabled
            };
            _UserInfoProvider.Set(newUser);

            // Generate new password, and save any other settings
            UserInfoProvider.SetPassword(newUser, password);

            return Task.FromResult(newUser.ToUser());
        }

        public async Task SendRegistrationConfirmationEmailAsync(User user, string confirmationUrl)
        {
            var AppUser = await _UserManager.FindByNameAsync(user.UserName);
            string token = await _UserManager.GenerateEmailConfirmationTokenAsync(AppUser);

            // Creates and sends the confirmation email to the user's address
            await _EmailService.SendEmailAsync(AppUser.Email, "Confirm your new account",
                string.Format($"Please confirm your new account by clicking <a href=\"{confirmationUrl}?userId={user.UserGUID}&token={HttpUtility.UrlEncode(token)}\">here</a>"));
        }

        public async Task<IdentityResult> ConfirmRegistrationConfirmationTokenAsync(User user, string token)
        {
            var AppUser = await _UserManager.FindByIdAsync(user.UserID.ToString());
            return await _UserManager.ConfirmEmailAsync(AppUser, token);
        }

        public async Task SendPasswordResetEmailAsync(User user, string confirmationLink)
        {
            var AppUser = await _UserManager.FindByIdAsync(user.UserID.ToString());
            string token = await _UserManager.GeneratePasswordResetTokenAsync(AppUser);

            // Creates and sends the confirmation email to the user's address
            await _EmailService.SendEmailAsync(user.Email, "Password Reset Request",
                $"A Password reset request has been generated for your account.  If you have generated this request, you may reset your password by clicking <a href=\"{confirmationLink}?userId={user.UserGUID}&token={HttpUtility.UrlEncode(token)}\">here</a>.");
        }

        public async Task<IdentityResult> ResetPasswordFromTokenAsync(User user, string token, string newPassword)
        {
            var AppUser = await _UserManager.FindByIdAsync(user.UserID.ToString());
            return await _UserManager.ResetPasswordAsync(AppUser, token, newPassword);
        }

        public async Task<bool> ValidateUserPasswordAsync(User user, string password)
        {
            var userInfoObj = await GetUserInfoAsync(user.UserName);
            return UserInfoProvider.ValidateUserPassword(userInfoObj, password);
        }

        public Task ResetPasswordAsync(User user, string password)
        {
            UserInfoProvider.SetPassword(user.UserName, password, true);
            return Task.CompletedTask;
        }

        public Task<bool> ValidatePasswordPolicyAsync(string password)
        {
            return Task.FromResult(SecurityHelper.CheckPasswordPolicy(password, _siteRepository.CurrentSiteName()));
        }

        private async Task<UserInfo> GetUserInfoAsync(string userName)
        {
            return await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency($"{UserInfo.OBJECT_TYPE}|byname|{userName}");
                }
                return await _UserInfoProvider.GetAsync(userName);
            }, new CacheSettings(15, "GetUserInfoAsync", userName));
        }

        public Task CreateExternalUserAsync(User user)
        {
            // Create basic user
            var newUser = new UserInfo()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                SiteIndependentPrivilegeLevel = UserPrivilegeLevelEnum.None,
                Enabled = user.Enabled,
                IsExternal = true
            };
            _UserInfoProvider.Set(newUser);

            return Task.FromResult(newUser.ToUser());
        }

        public async Task SendVerificationCodeEmailAsync(User user, string token)
        {
            // Creates and sends the confirmation email to the user's address
            await _EmailService.SendEmailAsync(user.Email, "Verification Code",
                $"<p>Hello {user.UserName}!</p><p>When Prompted, enter the code below to finish authenticating:</p> <table align=\"center\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td width=\"15%\"></td><td width=\"70%\" align=\"center\" bgcolor=\"#f1f3f2\" style=\"color:black;margin-bottom:10px;border-radius:10px\"><p style=\"font-size:xx-large;font-weight:bold;margin:10px 0px\">{token}</p></td></tr></tbody></table>");
        }
    }
}