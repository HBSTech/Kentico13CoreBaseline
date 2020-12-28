using MVCCaching;

namespace Generic.Services.Interfaces
{
    public interface IRoleService : IService
    {
        /// <summary>
        /// Sets the user's given role
        /// </summary>
        /// <param name="UserID">The User ID</param>
        /// <param name="RoleName">The Role Name</param>
        /// <param name="SiteName">The Site name</param>
        /// <param name="RoleToggle">True if they should be added to the role, false if removed</param>
        void SetUserRole(int UserID, string RoleName, string SiteName, bool RoleToggle);
    }
}