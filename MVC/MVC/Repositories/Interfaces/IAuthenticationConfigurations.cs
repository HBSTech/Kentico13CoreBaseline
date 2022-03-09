using Generic.Models.Account;
using System.Collections.Generic;

namespace Generic.Repositories.Interfaces
{
    public interface IAuthenticationConfigurations
    {
        ExistingInternalUserBehavior GetExistingInternalUserBehavior();
        IEnumerable<string> InternalUserRoles();
        IEnumerable<string> AllExternalUserRoles();
        IEnumerable<string> FacebookUserRoles();
        IEnumerable<string> GoogleUserRoles();
        IEnumerable<string> MicrosoftUserRoles();
        IEnumerable<string> TwitterUserRoles();
    }
}
