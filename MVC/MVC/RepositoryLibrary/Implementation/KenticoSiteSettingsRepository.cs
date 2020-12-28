using MVCCaching;
using Generic.Repositories.Interfaces;
using Generic.Models;
using CMS.DataEngine;
using Generic.Repositories.Helpers.Interfaces;

namespace Generic.Repositories.Implementations
{
    public class KenticoSiteSettingsRepository : ISiteSettingsRepository
    {
        private IKenticoSiteSettingsRepositoryHelper _Helper;

        public KenticoSiteSettingsRepository(IKenticoSiteSettingsRepositoryHelper Helper)
        {
            _Helper = Helper;
        }

        [DoNotCache]
        public PasswordPolicySettings GetPasswordPolicy(string SiteName)
        {
            return new PasswordPolicySettings()
            {
                UsePasswordPolicy = _Helper.GetBoolSettingValue(SiteName, "CMSUsePasswordPolicy"),
                MinLength = _Helper.GetIntSettingValue(SiteName, "CMSPolicyMinimalLength"),
                NumNonAlphanumericChars = _Helper.GetIntSettingValue(SiteName, "CMSPolicyNumberOfNonAlphaNumChars"),
                Regex = _Helper.GetStringSettingValue(SiteName, "CMSPolicyRegularExpression"),
                ViolationMessage = _Helper.GetStringSettingValue(SiteName, "CMSPolicyViolationMessage")
            };
        }
    }
}