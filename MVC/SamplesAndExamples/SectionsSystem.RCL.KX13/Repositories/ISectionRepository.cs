using SectionsSystem.Interfaces;

namespace SectionsSystem.Repositories
{
    public interface ISectionRepository
    {
        /// <summary>
        /// Gets all the Sections for the given parent page. 
        /// </summary>
        /// <param name="parentPage">The page that you want to show the sections for</param>
        /// <returns>A list of all the ISections</returns>
        Task<IEnumerable<ISection>> GetSectionsAsync(NodeIdentity parentPage);

        /// <summary>
        /// Gets the Section given the Section Identity.  It may be null if it isn't available in the current region.
        /// </summary>
        /// <param name="section">The Section Node Identity</param>
        /// <returns></returns>
        Task<Maybe<ISection>> GetSectionAsync(NodeIdentity section);

    }
}
