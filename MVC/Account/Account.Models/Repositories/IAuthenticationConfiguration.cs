namespace Account.Repositories
{

    public interface IAuthenticationConfigurations
    {
        /// <summary>
        /// How external users should be set when creating a user.
        /// </summary>
        /// <returns></returns>
        ExistingInternalUserBehavior GetExistingInternalUserBehavior();

        /// <summary>
        /// Gets the Internal User Roles code names
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> InternalUserRoles();

        /// <summary>
        /// Get all the external User Role code names
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> AllExternalUserRoles();

        /// <summary>
        /// Get User Role code names to be assigned to Facebook Users
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> FacebookUserRoles();

        /// <summary>
        /// Get User Role code names to be assigned to Google Users
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GoogleUserRoles();

        /// <summary>
        /// Get User Role code names to be assigned to Microsoft users
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> MicrosoftUserRoles();

        /// <summary>
        /// Get User Role code names to be assigned to Twitter users
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> TwitterUserRoles();

        /// <summary>
        /// If Two Form Auth is enabled
        /// </summary>
        /// <returns></returns>
        bool UseTwoFormAuthentication();
    }
}
