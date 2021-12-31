using Generic.Models;
using MVCCaching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface ITabRepository : IRepository
    {
        /// <summary>
        /// Gets the Tab Parent by nodeID
        /// </summary>
        /// <param name="nodeID"The NodeID</param>
        /// <returns></returns>
        Task<TabParentItem> GetTabParentAsync(int nodeID);

        /// <summary>
        /// Gets all the Tabs found under the given path (usually the path of the Parent)
        /// </summary>
        /// <param name="path">The path where the tabs lay under.</param>
        /// <returns>The Tabs</returns>
        Task<IEnumerable<TabItem>> GetTabsAsync(string path);
    }
}
