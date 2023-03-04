namespace Account.Repositories
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Gets the given role
        /// </summary>
        /// <param name="roleName">The Role Name</param>
        /// <param name="siteName">The Site Name</param>
        /// <returns>The Role</returns>
        Task<Result<RoleItem>> GetRoleAsync(string roleName, string siteName);

        /// <summary>
        /// Returns if the user is in the given role
        /// </summary>
        /// <param name="userID">The User ID</param>
        /// <param name="roleName">The Role Name</param>
        /// <param name="siteName">The Site Name</param>
        /// <returns>If the user is in the role</returns>
        Task<bool> UserInRoleAsync(int userID, string roleName, string siteName);

        /// <summary>
        /// Checks if the current user has permission to the given Resource and Permission
        /// </summary>
        /// <param name="userID">The User ID</param>
        /// <param name="resourceName">The Resource Code Name</param>
        /// <param name="permissionName">The Permission Name</param>
        /// <param name="siteName">The Site Name</param>
        /// <returns>If they have this permission</returns>
        Task<bool> UserHasPermissionAsync(int userID, string resourceName, string permissionName, string siteName);
    }
}