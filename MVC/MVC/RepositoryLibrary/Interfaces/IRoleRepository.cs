using CMS.Membership;
using MVCCaching;

namespace Generic.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository
    {
        /// <summary>
        /// Gets the given role
        /// </summary>
        /// <param name="RoleName">The Role Name</param>
        /// <param name="SiteName">The Site Name</param>
        /// <param name="Columns">Columns needed</param>
        /// <returns>The Role</returns>
        RoleInfo GetRole(string RoleName, string SiteName, string[] Columns = null);

        /// <summary>
        /// Returns if the user is in the given role
        /// </summary>
        /// <param name="UserID">The User ID</param>
        /// <param name="RoleName">The Role Name</param>
        /// <param name="SiteName">The Site Name</param>
        /// <returns>If the user is in the role</returns>
        bool UserInRole(int UserID, string RoleName, string SiteName);

        /// <summary>
        /// Sets the user's given role
        /// </summary>
        /// <param name="UserID">The User ID</param>
        /// <param name="RoleName">The Role Name</param>
        /// <param name="SiteName">The Site name</param>
        /// <param name="RoleToggle">True if they should be added to the role, false if removed</param>
        void SetUserRole(int UserID, string RoleName, string SiteName, bool RoleToggle);

        /// <summary>
        /// Checks if the current user has permission to the given Resource and Permission
        /// </summary>
        /// <param name="UserID">The User ID</param>
        /// <param name="ResourceName">The Resource Code Name</param>
        /// <param name="PermissionName">The Permission Name</param>
        /// <param name="SiteName">The Site Name</param>
        /// <returns>If they have this permission</returns>
        bool UserHasPermission(int UserID, string ResourceName, string PermissionName, string SiteName);
    }
}