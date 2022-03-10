using Generic.Models.Account;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Generic.Repositories.Implementation
{
    public class BaselineAuthenticationConfiguration : IAuthenticationConfigurations
    {
        public BaselineAuthenticationConfiguration()
        {

        }
        public ExistingInternalUserBehavior ExistingInternalUserBehavior { get; set; } = ExistingInternalUserBehavior.LeaveAsIs;
        public List<string> InternalUserRoles { get; set; } = new List<string>();
        public List<string> AllExternalUserRoles { get; set; } = new List<string>();
        public List<string> FacebookUserRoles { get; set; } = new List<string>();
        public List<string> GoogleUserRoles { get; set; } = new List<string>();
        public List<string> MicrosoftUserRoles { get; set; } = new List<string>();
        public List<string> TwitterUserRoles { get; set; } = new List<string>();
        public bool UseTwoFormAuthentication { get; set; } = false;

        public ExistingInternalUserBehavior GetExistingInternalUserBehavior() => ExistingInternalUserBehavior;

        IEnumerable<string> IAuthenticationConfigurations.AllExternalUserRoles() => AllExternalUserRoles;

        IEnumerable<string> IAuthenticationConfigurations.FacebookUserRoles() => FacebookUserRoles;

        IEnumerable<string> IAuthenticationConfigurations.GoogleUserRoles() => GoogleUserRoles;

        IEnumerable<string> IAuthenticationConfigurations.InternalUserRoles() => InternalUserRoles;

        IEnumerable<string> IAuthenticationConfigurations.MicrosoftUserRoles() => MicrosoftUserRoles;

        IEnumerable<string> IAuthenticationConfigurations.TwitterUserRoles() => TwitterUserRoles;

        bool IAuthenticationConfigurations.UseTwoFormAuthentication() => UseTwoFormAuthentication;
    }

    public static class AuthenticationConfigurationExtensions
    {
        public static AuthenticationBuilder ConfigureAuthentication(this AuthenticationBuilder builder, Action<BaselineAuthenticationConfiguration> configuration)
        {
            var defaultObj = new BaselineAuthenticationConfiguration()
            {
                // default here
                AllExternalUserRoles = new List<string>() { "external-user" }
            };
            configuration.Invoke(defaultObj);
            builder.Services.AddSingleton<IAuthenticationConfigurations>(defaultObj);
            return builder;
        }
    }
}
