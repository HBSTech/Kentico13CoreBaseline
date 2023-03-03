namespace Core.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets the current User (Public if not found)
        /// </summary>
        /// <returns></returns>
        Task<User> GetCurrentUserAsync();

        /// <summary>
        /// Gets the given user by user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<Result<User>> GetUserAsync(int userID);

        /// <summary>
        /// Gets the given user by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<Result<User>> GetUserAsync(string userName);

        /// <summary>
        /// Gets the given user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Result<User>> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets the given user by user GUID
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        Task<Result<User>> GetUserAsync(Guid userGuid);

        /// <summary>
        /// Checks the request header and or query parameters for the given user code, then returns the given user (or current user if not found), used in external API authentication calls
        /// </summary>
        /// <param name="headerParameter"></param>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        Task<User> GetUserByAuthenticationCodeAsync(string headerParameter, string queryParameter);

        /// <summary>
        /// Gets the user by the given user authentication code, (or current user if not found), used for user hash calls
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        Task<User> GetUserByAuthenticationCodeAsync(string userCode);
    }
}
