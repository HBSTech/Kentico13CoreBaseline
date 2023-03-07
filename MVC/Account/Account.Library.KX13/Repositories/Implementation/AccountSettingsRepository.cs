using CMS.DataEngine;
using Core.Services;
using MVCCaching;
using MVCCaching.Base.Core.Interfaces;

namespace Account.Repositories.Implementation
{
    internal class AccountSettingsRepository : IAccountSettingsRepository
    {
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ISiteRepository _siteRepository;
        private readonly IUrlResolver _urlResolver;

        public AccountSettingsRepository(ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ISiteRepository siteRepository,
            IUrlResolver urlResolver)
        {
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _siteRepository = siteRepository;
            _urlResolver = urlResolver;
        }


        public Task<string> GetAccountConfirmationUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "BlogArticlesToDAccountConfirmationUrlisplay");
            string url = SettingsKeyInfoProvider.GetValue("AccountConfirmationUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountForgottenPasswordResetUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountForgottenPasswordResetUrl");
            string url = SettingsKeyInfoProvider.GetValue("AccountForgottenPasswordResetUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountForgotPasswordUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountForgotPasswordUrl");
            string url = SettingsKeyInfoProvider.GetValue("AccountForgotPasswordUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountLoginUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountLoginUrl");
            string url = SettingsKeyInfoProvider.GetValue("AccountLoginUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountRegistrationUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountRegistrationUrl");
            string url = SettingsKeyInfoProvider.GetValue("AccountRegistrationUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountMyAccountUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountMyAccountUrl");
            string url = SettingsKeyInfoProvider.GetValue("AccountMyAccountUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<bool> GetAccountRedirectToAccountAfterLoginAsync()
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountRedirectToAccountAfterLogin");
            return Task.FromResult(SettingsKeyInfoProvider.GetBoolValue("AccountRedirectToAccountAfterLogin", _siteRepository.CurrentSiteName()));
        }

        public Task<string> GetAccountLogOutUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountLogOutUrl");
            string url = SettingsKeyInfoProvider.GetValue("AccountLogOutUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountResetPasswordUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccountResetPassword");
            string url = SettingsKeyInfoProvider.GetValue("AccountResetPassword", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccessDeniedUrlAsync(string fallBackUrl)
        {
            _ = _cacheDependencyBuilderFactory.Create()
                .Object(SettingsKeyInfo.OBJECT_TYPE, "AccessDeniedUrl");
            string url = SettingsKeyInfoProvider.GetValue("AccessDeniedUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<PasswordPolicySettings> GetPasswordPolicyAsync()
        {
            return Task.FromResult(GetPasswordPolicy());
        }

        public PasswordPolicySettings GetPasswordPolicy()
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Object(SettingsKeyInfo.OBJECT_TYPE, "CMSUsePasswordPolicy")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyMinimalLength")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyNumberOfNonAlphaNumChars")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyRegularExpression")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyViolationMessage");

            var siteName = _siteRepository.CurrentSiteName();

            // Kentico has own internal caching of settings so no need to cache further.
            var passwordPolicy = new PasswordPolicySettings(
                usePasswordPolicy: SettingsKeyInfoProvider.GetBoolValue("CMSUsePasswordPolicy", siteName),
                minLength: SettingsKeyInfoProvider.GetIntValue("CMSPolicyMinimalLength", siteName),
                numNonAlphanumericChars: SettingsKeyInfoProvider.GetIntValue("CMSPolicyNumberOfNonAlphaNumChars", siteName),
                regex: SettingsKeyInfoProvider.GetValue("CMSPolicyRegularExpression", siteName),
                violationMessage: SettingsKeyInfoProvider.GetValue("CMSPolicyViolationMessage", siteName)
                );

            return passwordPolicy;
        }
    }
}
