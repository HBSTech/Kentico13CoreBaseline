using CMS.Base;
using CMS.Membership;
using MVCCaching;
using System;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IUserRepository : IRepository
    {
        /// <summary>
        /// Gets the UserID based on the GUID
        /// </summary>
        /// <param name="UserGuid">The User GUID</param>
        /// <returns>The UserID, null if not found</returns>
        int? UserGuidToID(Guid UserGuid);

        /// <summary>
        /// Gets the user by the given Username
        /// </summary>
        /// <param name="Username">The Username</param>
        /// <param name="Columns">What fields you wish to retrieve</param>
        /// <returns>The User</returns>
        IUserInfo GetUserByUsername(string Username, string[] Columns = null);

        /// <summary>
        /// Gets the user by email
        /// </summary>
        /// <param name="Email">The user's email address</param>
        /// <param name="Columns">The fields you wish to retrieve</param>
        /// <returns>The User</returns>
        IUserInfo GetUserByEmail(string Email, string[] Columns = null);

        /// <summary>
        /// Gets the user by ID
        /// </summary>
        /// <param name="UserID">The user's ID</param>
        /// <param name="Columns">The fields you wish to retrieve</param>
        /// <returns>The User</returns>
        IUserInfo GetUserByID(int UserID, string[] Columns = null);

        /// <summary>
        /// Gets the IUserInfo based on the GUID
        /// </summary>
        /// <param name="UserGuid">The User GUID</param>
        /// <returns>The IUserInfo, null if not found</returns>
        public IUserInfo GetUserByGuid(Guid UserGuid);
    }
}