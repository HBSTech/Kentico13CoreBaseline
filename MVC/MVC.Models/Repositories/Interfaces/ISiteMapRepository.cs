using Generic.Models;
using MVCCaching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface ISiteMapRepository : IRepository
    {
        /// <summary>
        /// Gets the SiteMapUrlSet given the SiteMapOptions
        /// </summary>
        /// <param name="options">The SiteMapOptions you wish to pass to get your SitemapNodes</param>
        /// <returns>List of all the SitemapNodes</returns>
        Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetAsync(SiteMapOptions options);

        /// <summary>
        /// Gets the Site map nodes using default settings.  In Kentico this is through the 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SitemapNode>> GetSiteMapUrlSetAsync();
    }
}