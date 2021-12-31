using Generic.Models;
using MVCCaching;
using System;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IPageContextRepository : IRepository
    {
        /// <summary>
        /// If the current page is in edit mode
        /// </summary>
        /// <returns></returns>
        Task<bool> IsEditModeAsync();

        Task<PageIdentity> GetCurrentPageAsync();

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="path">Path (NodeAliasPath)</param>
        /// <returns>The Page Identity or null if not found</returns>
        Task<PageIdentity> GetPageAsync(string path);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="documentID">Document ID</param>
        /// <returns>The Page Identity or null if not found</returns>
        Task<PageIdentity> GetPageAsync(int documentID);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="documentGUID">Document GUID</param>
        /// <returns>The Page Identity or null if not found</returns>
        Task<PageIdentity> GetPageAsync(Guid documentGUID);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="nodeID">Node ID</param>
        /// <returns>The Page Identity or null if not found</returns>
        Task<PageIdentity> GetPageByNodeAsync(int nodeID);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="nodeGUID">Node GUID</param>
        /// <returns>The Page Identity or null if not found</returns>
        Task<PageIdentity> GetPageByNodeAsync(Guid nodeGUID);
    }
}
