using CMS.DataEngine;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;

namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoSiteSettingsRepositoryHelper : IKenticoSiteSettingsRepositoryHelper
    {
        [CacheDependency("cms.sitesetting|byname|{1}")]
        public string GetStringSettingValue(string SiteName, string SettingsKeyCode)
        {
            return SettingsKeyInfoProvider.GetValue(SettingsKeyCode, new SiteInfoIdentifier(SiteName));
        }

        [CacheDependency("cms.sitesetting|byname|{1}")]
        public bool GetBoolSettingValue(string SiteName, string SettingsKeyCode, bool? DefaultValue = null)
        {
            if (DefaultValue.HasValue)
            {
                return SettingsKeyInfoProvider.GetBoolValue(SettingsKeyCode, new SiteInfoIdentifier(SiteName), DefaultValue.Value);
            }
            else
            {
                return SettingsKeyInfoProvider.GetBoolValue(SettingsKeyCode, new SiteInfoIdentifier(SiteName));
            }
        }

        [CacheDependency("cms.sitesetting|byname|{1}")]
        public decimal GetDecimalSettingValue(string SiteName, string SettingsKeyCode)
        {
            return SettingsKeyInfoProvider.GetDecimalValue(SettingsKeyCode, new SiteInfoIdentifier(SiteName));
        }

        [CacheDependency("cms.sitesetting|byname|{1}")]
        public double GetDoubleSettingValue(string SiteName, string SettingsKeyCode)
        {
            return SettingsKeyInfoProvider.GetDoubleValue(SettingsKeyCode, new SiteInfoIdentifier(SiteName));
        }

        [CacheDependency("cms.sitesetting|byname|{1}")]
        public int GetIntSettingValue(string SiteName, string SettingsKeyCode)
        {
            return SettingsKeyInfoProvider.GetIntValue(SettingsKeyCode, new SiteInfoIdentifier(SiteName));
        }
    }
}