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
    }
}
