using CMS.Base;
using CMS.Membership;
using Generic.Models.User;
using Microsoft.AspNetCore.Identity;
using MVCCaching;
using System.Threading.Tasks;

namespace Generic.Services.Interfaces
{
    public interface IUserService : IService
    {

        /// <summary>
        /// Converts a BasicUser to an IUserInfo type
        /// </summary>
        /// <param name="User">The Basic User</param>
        /// <returns>an IUserInfo object</returns>
        UserInfo BasicUserToIUser(BasicUser User);

        /// <summary>
        /// Creates a user on the website.
        /// </summary>
        /// <param name="User">The User information</param>
        /// <param name="Password">The Password</param>
        /// <param name="Enabled">If they should be enabled right away</param>
        /// <returns>The UserInfo Object</returns>
        UserInfo CreateUser(IUserInfo User, string Password, bool Enabled = false);

        /// <summary>
        /// Sends a Registration Confirmation Email Asyncly to the given User, with the Confirmation link provided
        /// </summary>
        /// <param name="user">The User object</param>
        /// <param name="ConfirmationLink">The base URL for the Email Confirmation string, the user GUID and Hash are appended to this</param>
        /// <returns></returns>
        Task SendRegistrationConfirmationEmailAsync(IUserInfo user, string ConfirmationLink);

        /// <summary>
        /// Validates the Token request for the given User
        /// </summary>
        /// <param name="UserID">The User's ID</param>
        /// <param name="token">The Token</param>
        /// <returns>If the token is valid</returns>
        Task<IdentityResult> ConfirmRegistrationConfirmationToken(int UserID, string token);

        /// <summary>
        /// Sends a password reset email for the given user
        /// </summary>
        /// <param name="user">The User object</param>
        /// <param name="ConfirmationLink">The base URL for the Email Confirmation string, the user GUID and Hash are appended to this</param>
        /// <returns></returns>
        Task SendPasswordResetEmailAsync(IUserInfo user, string ConfirmationLink);

        /// <summary>
        /// Validates and resets the password for the given user and token
        /// </summary>
        /// <param name="UserID">The User's ID</param>
        /// <param name="Token">The Token</param>
        /// <param name="NewPassword">The new password</param>
        /// <returns>If the operation was successful</returns>
        Task<IdentityResult> ResetPasswordFromToken(int UserID, string Token, string NewPassword);

        /// <summary>
        /// Validates if the password is valid for the given user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="password">the password</param>
        /// <returns>If it's the correct password</returns>
        bool ValidateUserPassword(IUserInfo user, string password);

        /// <summary>
        /// Resets the password of the given user
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="password">The password to reset it to</param>
        void ResetPassword(IUserInfo user, string password);

        /// <summary>
        /// Validates that the given password passes the site's Password Policy
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ValidatePasswordPolicy(string password, string SiteName);
    }
}