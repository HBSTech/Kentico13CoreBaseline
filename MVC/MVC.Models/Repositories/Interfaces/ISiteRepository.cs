using MVCCaching;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface ISiteRepository : IRepository
    {
        /// <summary>
        /// Gets the SiteID for the given SiteName
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <returns>The SiteID</returns>
        Task<int> GetSiteIDAsync(string siteName = null);

        /// <summary>
        /// Gets the current Site Name, synchronously available as it is usually derived from the current url and no database call needed.
        /// </summary>
        /// <returns>The Site Name</returns>
        string CurrentSiteName();

        /// <summary>
        /// Gets the current Site Name
        /// </summary>
        /// <returns>The Site Name</returns>
        Task<string> CurrentSiteNameAsync();

    }
}