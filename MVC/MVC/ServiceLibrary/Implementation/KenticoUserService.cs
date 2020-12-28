using AutoMapper;
using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.Synchronization;
using Generic.Models.User;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Kentico.Membership;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;

namespace Generic.Services.Implementation
{
    public class KenticoUserService : IUserService
    {
        private readonly ApplicationUserManager<ApplicationUser> _UserManager;
        private readonly IMapper _Mapper;
        private readonly IUserInfoProvider _UserInfoProvider;
        private readonly IMessageService _EmailService;

        public KenticoUserService(
            IMapper Mapper,
            IUserInfoProvider UserInfoProvider,
            ApplicationUserManager<ApplicationUser> userManager,
            IMessageService emailService)
        {
            _UserManager = userManager;
            _Mapper = Mapper;
            _UserInfoProvider = UserInfoProvider;
            _EmailService = emailService;
        }

        public UserInfo CreateUser(IUserInfo User, string Password, bool Enabled = false)
        {
            // Create basic user
            UserInfo NewUser = new UserInfo()
            {
                UserName = User.UserName,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email,
                SiteIndependentPrivilegeLevel = UserPrivilegeLevelEnum.None,
                Enabled = Enabled
            };
            _UserInfoProvider.Set(NewUser);

            // Generate new password, and save any other settings
            UserInfoProvider.SetPassword(NewUser, Password);

            return NewUser;
        }

        public async Task SendRegistrationConfirmationEmailAsync(IUserInfo user, string confirmationUrl)
        {
            var AppUser = await _UserManager.FindByNameAsync(user.UserName);
            string token = await _UserManager.GenerateEmailConfirmationTokenAsync(AppUser);

            // Creates and sends the confirmation email to the user's address
            await _EmailService.SendEmailAsync(AppUser.Email, "Confirm your new account",
                string.Format($"Please confirm your new account by clicking <a href=\"{confirmationUrl}?userId={user.UserGUID}&token={HttpUtility.UrlEncode(token)}\">here</a>"));
        }

        public async Task<IdentityResult> ConfirmRegistrationConfirmationToken(int UserID, string token)
        {
            var AppUser = await _UserManager.FindByIdAsync(UserID.ToString());
            return await _UserManager.ConfirmEmailAsync(AppUser, token);
        }

        public async Task SendPasswordResetEmailAsync(IUserInfo user, string ConfirmationLink)
        {
            var AppUser = await _UserManager.FindByIdAsync(user.UserID.ToString());
            string token = await _UserManager.GeneratePasswordResetTokenAsync(AppUser);

            // Creates and sends the confirmation email to the user's address
            await _EmailService.SendEmailAsync(user.Email, "Password Reset Request",
                string.Format($"A Password reset request has been generated for your account.  If you have generated this request, you may reset your password by clicking <a href=\"{ConfirmationLink}?userId={user.UserGUID}&token={HttpUtility.UrlEncode(token)}\">here</a>."));
        }

        public async Task<IdentityResult> ResetPasswordFromToken(int UserID, string Token, string NewPassword)
        {
            var AppUser = await _UserManager.FindByIdAsync(UserID.ToString());
            return await _UserManager.ResetPasswordAsync(AppUser, Token, NewPassword);
        }

        public bool ValidateUserPassword(IUserInfo user, string password)
        {
            return UserInfoProvider.ValidateUserPassword(user as UserInfo, password);
        }

        public void ResetPassword(IUserInfo user, string password)
        {
            UserInfoProvider.SetPassword(user.UserName, password, true);
        }

        public bool ValidatePasswordPolicy(string password, string SiteName)
        {
            return SecurityHelper.CheckPasswordPolicy(password, SiteName);
        }

        public UserInfo BasicUserToIUser(BasicUser User)
        {
            return _Mapper.Map<UserInfo>(User);
        }
    }
}