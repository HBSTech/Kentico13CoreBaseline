using Generic.Repositories.Interfaces;
using Generic.Libraries.Helpers;
using Generic.Models;
using System.Threading.Tasks;
using MVCCaching.Base.Core.Interfaces;
using CMS.DataEngine;
using Microsoft.AspNetCore.Mvc;
using Generic.Services.Interfaces;

namespace Generic.Repositories.Implementations
{
    public class SiteSettingsRepository : ISiteSettingsRepository
    {
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteRepository _siteRepository;
        private readonly IUrlResolver _urlResolver;

        public SiteSettingsRepository(ICacheDependenciesStore cacheDependenciesStore,
            ISiteRepository siteRepository,
            IUrlResolver urlResolver)
        {
            _cacheDependenciesStore = cacheDependenciesStore;
            _siteRepository = siteRepository;
            _urlResolver = urlResolver;
        }


        public Task<string> GetAccountConfirmationUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountConfirmationUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountForgottenPasswordResetUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountForgottenPasswordResetUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountForgotPasswordUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountForgotPasswordUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountLoginUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountLoginUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountRegistrationUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountRegistrationUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountMyAccountUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountMyAccountUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<bool> GetAccountRedirectToAccountAfterLoginAsync()
        {
            return Task.FromResult(SettingsKeyInfoProvider.GetBoolValue("AccountRedirectToAccountAfterLogin", _siteRepository.CurrentSiteName()));
        }

        public Task<string> GetAccountLogOutUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountLogOutUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccountResetPasswordUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccountResetPassword", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<string> GetAccessDeniedUrlAsync(string fallBackUrl)
        {
            string url = SettingsKeyInfoProvider.GetValue("AccessDeniedUrl", _siteRepository.CurrentSiteName());
            url = !string.IsNullOrWhiteSpace(url) ? url : fallBackUrl;
            return Task.FromResult(_urlResolver.ResolveUrl(url));
        }

        public Task<PasswordPolicySettings> GetPasswordPolicyAsync()
        {
            return Task.FromResult(GetPasswordPolicy());
        }

        public Task<string> GetImageUploadMediaLibraryAsync()
        {
            string libraryName = SettingsKeyInfoProvider.GetValue("ImageUploaderDefaultMediaLibrary", _siteRepository.CurrentSiteName());
            libraryName = !string.IsNullOrWhiteSpace(libraryName) ? libraryName : "Graphics";
            return Task.FromResult(libraryName);
        }

        public PasswordPolicySettings GetPasswordPolicy()
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(SettingsKeyInfo.OBJECT_TYPE, "CMSUsePasswordPolicy")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyMinimalLength")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyNumberOfNonAlphaNumChars")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyRegularExpression")
                .Object(SettingsKeyInfo.OBJECT_TYPE, "CMSPolicyViolationMessage");

            var siteName = _siteRepository.CurrentSiteName();

            // Kentico has own internal caching of settings so no need to cache further.
            var passwordPolicy = new PasswordPolicySettings()
            {
                UsePasswordPolicy = SettingsKeyInfoProvider.GetBoolValue("CMSUsePasswordPolicy", siteName),
                MinLength = SettingsKeyInfoProvider.GetIntValue("CMSPolicyMinimalLength", siteName),
                NumNonAlphanumericChars = SettingsKeyInfoProvider.GetIntValue("CMSPolicyNumberOfNonAlphaNumChars", siteName),
                Regex = SettingsKeyInfoProvider.GetValue("CMSPolicyRegularExpression", siteName),
                ViolationMessage = SettingsKeyInfoProvider.GetValue("CMSPolicyViolationMessage", siteName)
            };

            return passwordPolicy;
        }
    }
}