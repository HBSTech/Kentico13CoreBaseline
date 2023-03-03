namespace Core.Repositories
{
    public interface ISiteRepository
    {
        /// <summary>
        /// Gets the SiteID for the given SiteName
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <returns>The SiteID</returns>
        Task<int> GetSiteIDAsync(string? siteName = null);

        /// <summary>
        /// Gets the current Site Name, synchronously available as it is usually derived from the current url and no database call needed.
        /// </summary>
        /// <returns>The Site Name</returns>
        string CurrentSiteName();

        /// <summary>
        /// Gets the current Site Display name, synchronously available as it is usually derived from the current url and no database call needed. 
        /// </summary>
        /// <returns></returns>
        string CurrentSiteDisplayName();

        /// <summary>
        /// Gets the current Site Name
        /// </summary>
        /// <returns>The Site Name</returns>
        Task<string> CurrentSiteNameAsync();

        /// <summary>
        /// Gets the current Site ID
        /// </summary>
        /// <returns></returns>
        int CurrentSiteID();

        /// <summary>
        /// Gets the site name (not lowerecased, just as is) for the given SiteID.  Empty string if not found.
        /// 
        /// Synchronous as cached and rarely changes but referenced a lot in requests.
        /// </summary>
        /// <param name="siteID">The SiteID</param>
        /// <returns>The SiteName</returns>
		string SiteNameById(int siteID);
	}
}