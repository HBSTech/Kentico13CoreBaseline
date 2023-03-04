namespace Account.Services
{
    public interface IRoleService
    {
        /// <summary>
        /// Sets the user's given role
        /// </summary>
        /// <param name="userID">The User ID</param>
        /// <param name="roleName">The Role Name</param>
        /// <param name="siteName">The Site name</param>
        /// <param name="roleToggle">True if they should be added to the role, false if removed</param>
        Task SetUserRole(int userID, string roleName, string siteName, bool roleToggle);
        
        /// <summary>
        /// Creates the role if it doesn't already exist.
        /// </summary>
        /// <param name="roleName">Role Name</param>
        /// <param name="siteName">Site Name</param>
        /// <returns></returns>
        Task CreateRoleIfNotExisting(string roleName, string siteName);
    }
}