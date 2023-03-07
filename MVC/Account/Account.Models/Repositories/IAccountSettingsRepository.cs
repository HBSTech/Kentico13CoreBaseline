namespace Account.Repositories
{
    /// <summary>
    /// Add Site Settings retrieval methods here
    /// </summary>
    public interface IAccountSettingsRepository
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

        /// <summary>
        /// Get Account Confirmation URL
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountConfirmationUrlAsync(string fallBackUrl);

        /// <summary>
        /// Get account Registration Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountRegistrationUrlAsync(string fallBackUrl);

        /// <summary>
        /// Get Account Login Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountLoginUrlAsync(string fallBackUrl);

        /// <summary>
        /// Get Account Log out Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountLogOutUrlAsync(string fallBackUrl);

        /// <summary>
        /// Get My Account Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountMyAccountUrlAsync(string fallBackUrl);

        /// <summary>
        /// If the account should redirect to their My Account after logging in
        /// </summary>
        /// <returns></returns>
        Task<bool> GetAccountRedirectToAccountAfterLoginAsync();

        /// <summary>
        /// Get the Forgot Password Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountForgotPasswordUrlAsync(string fallBackUrl);

        /// <summary>
        /// Get the Forgotten Password Reset Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountForgottenPasswordResetUrlAsync(string fallBackUrl);

        /// <summary>
        /// Get the Account Reset Password Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccountResetPasswordUrlAsync(string fallBackUrl);

        /// <summary>
        /// Get the Access Denied Url
        /// </summary>
        /// <param name="fallBackUrl"></param>
        /// <returns></returns>
        Task<string> GetAccessDeniedUrlAsync(string fallBackUrl);

    }
}