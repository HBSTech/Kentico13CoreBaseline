using Generic.Models;
using MVCCaching;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    /// <summary>
    /// Add Site Settings retrieval methods here
    /// </summary>
    public interface ISiteSettingsRepository : IRepository
    {
        /// <summary>
        /// Non Async version specifically used in Attribute Validation for Fluent, as often need it on outside
        /// </summary>
        /// <returns></returns>
        PasswordPolicySettings GetPasswordPolicy();

        /// <summary>
        /// Gets the password policy based on the settings
        /// </summary>
        /// <returns>The Password Policy Settings</returns>
        Task<PasswordPolicySettings> GetPasswordPolicyAsync();
        Task<string> GetAccountConfirmationUrlAsync(string fallBackUrl);
        Task<string> GetAccountRegistrationUrlAsync(string fallBackUrl);
        Task<string> GetAccountLoginUrlAsync(string fallBackUrl);
        Task<string> GetAccountLogOutUrlAsync(string fallBackUrl);
        Task<string> GetAccountMyAccountUrlAsync(string fallBackUrl);
        Task<bool> GetAccountRedirectToAccountAfterLoginAsync();
        Task<string> GetAccountForgotPasswordUrlAsync(string fallBackUrl);
        Task<string> GetAccountForgottenPasswordResetUrlAsync(string fallBackUrl);
        Task<string> GetAccountResetPasswordUrlAsync(string fallBackUrl);
        Task<string> GetAccessDeniedUrlAsync(string fallBackUrl);
        Task<string> GetImageUploadMediaLibraryAsync();

        // Custom Methods here
    }
}