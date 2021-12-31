using Generic.Models;
using MVCCaching;
using System;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IUserRepository : IRepository
    {

        Task<User> GetCurrentUserAsync();

        Task<User> GetUserAsync(int userID);

        Task<User> GetUserAsync(string userName);

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserAsync(Guid userGuid);
    }
}
