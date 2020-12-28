using Generic.Models;
using MVCCaching;
using System.Collections.Generic;

namespace Generic.Repositories.Interfaces
{
    public interface ISiteMapRepository : IRepository
    {
        /// <summary>
        /// Gets the SiteMapUrlSet given the SiteMapOptions
        /// </summary>
        /// <param name="Options">The SiteMapOptions you wish to pass to get your SitemapNodes</param>
        /// <returns>List of all the SitemapNodes</returns>
        IEnumerable<SitemapNode> GetSiteMapUrlSet(SiteMapOptions Options);
    }
}