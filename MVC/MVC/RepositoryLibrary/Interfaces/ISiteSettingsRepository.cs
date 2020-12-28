using Generic.Models;
using MVCCaching;

namespace Generic.Repositories.Interfaces
{
    /// <summary>
    /// Add Site Settings retrieval methods here
    /// </summary>
    public interface ISiteSettingsRepository : IRepository
    {
        /// <summary>
        /// Gets the password policy based on the settings
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <returns>The Password Policy Settings</returns>
        PasswordPolicySettings GetPasswordPolicy(string SiteName);

        // Custom Methods here

    }
}