using MVCCaching;

namespace Generic.Repositories.Helpers.Interfaces
{
    /// <summary>
    /// Helper for the KenticoSiteSettingsRepository
    /// </summary>
    public interface IKenticoSiteSettingsRepositoryHelper : IRepository
    {
        /// <summary>
        /// Gets the string Settings Value
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="SettingsKeyCode">The Settings Key</param>
        /// <returns>The Settings Value</returns>
        string GetStringSettingValue(string SiteName, string SettingsKeyCode);

        /// <summary>
        /// Gets the bool Settings Value
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="SettingsKeyCode">The Settings Key</param>
        /// <param name="DefaultValue">If not set, what the default value is</param>
        /// <returns>The Settings Value</returns>
        bool GetBoolSettingValue(string SiteName, string SettingsKeyCode, bool? DefaultValue = null);

        /// <summary>
        /// Gets the decimal Settings Value
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="SettingsKeyCode">The Settings Key</param>
        /// <returns>The Settings Value</returns>
        decimal GetDecimalSettingValue(string SiteName, string SettingsKeyCode);

        /// <summary>
        /// Gets the double Settings Value
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="SettingsKeyCode">The Settings Key</param>
        /// <returns>The Settings Value</returns>
        double GetDoubleSettingValue(string SiteName, string SettingsKeyCode);

        /// <summary>
        /// Gets the int Settings Value
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="SettingsKeyCode">The Settings Key</param>
        /// <returns>The Settings Value</returns>
        int GetIntSettingValue(string SiteName, string SettingsKeyCode);
    }
}