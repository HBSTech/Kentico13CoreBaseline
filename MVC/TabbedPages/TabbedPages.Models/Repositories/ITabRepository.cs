namespace TabbedPages.Repositories
{
    public interface ITabRepository
    {
        /// <summary>
        /// Gets all the Tabs found under the given path (usually the path of the Parent)
        /// </summary>
        /// <param name="path">The path where the tabs lay under.</param>
        /// <returns>The Tabs</returns>
        Task<IEnumerable<TabItem>> GetTabsAsync(string path);
        Task<IEnumerable<TabItem>> GetTabsAsync(string path, string siteName);
    }
}
