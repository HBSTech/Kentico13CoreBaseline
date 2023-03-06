using Core.Models;

namespace TabbedPages.Repositories
{
    public interface ITabRepository
    {
        /// <summary>
        /// Gets all the Tabs found under the given path (usually the path of the Parent)
        /// </summary>
        /// <param name="parentIdentity">The parent that the tabs reside under.</param>
        /// <returns>The Tabs</returns>
        Task<IEnumerable<TabItem>> GetTabsAsync(NodeIdentity parentIdentity);
    }
}
