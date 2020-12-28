using CMS.Membership;
using MVCCaching;

namespace Generic.Repositories.Helpers.Interfaces
{
    public interface IKenticoRoleRepositoryHelper : IRepository
    {
        /// <summary>
        /// Gets the UserInfo by UserID
        /// </summary>
        /// <param name="UserID">The UserID</param>
        /// <returns>The User</returns>
        UserInfo GetUser(int UserID);

        /// <summary>
        /// Gets the Roles
        /// </summary>
        /// <param name="RoleName">The Role Name</param>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="Columns">What columns in the Role object you need</param>
        /// <returns>The Role</returns>
        RoleInfo GetRole(string RoleName, string SiteName, string[] Columns = null);
    }
}