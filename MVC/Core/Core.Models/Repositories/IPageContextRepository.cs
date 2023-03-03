namespace Core.Repositories
{
    public interface IPageContextRepository
    {
        /// <summary>
        /// If the current page is in edit mode
        /// </summary>
        /// <returns></returns>
        Task<bool> IsEditModeAsync();

        /// <summary>
        /// Gets the current Page Identity if the request was derived from routing
        /// </summary>
        /// <returns></returns>
        Task<Result<PageIdentity>> GetCurrentPageAsync();

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="path">Path (NodeAliasPath)</param>
        /// <returns>The Page Identity</returns>
        Task<Result<PageIdentity>> GetPageAsync(string path);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="documentID">Document ID</param>
        /// <returns>The Page Identity </returns>
        Task<Result<PageIdentity>> GetPageAsync(int documentID);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="documentGUID">Document GUID</param>
        /// <returns>The Page Identity</returns>
        Task<Result<PageIdentity>> GetPageAsync(Guid documentGUID);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="nodeID">Node ID</param>
        /// <returns>The Page Identity</returns>
        Task<Result<PageIdentity>> GetPageByNodeAsync(int nodeID);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="nodeGUID">Node GUID</param>
        /// <returns>The Page Identity</returns>
        Task<Result<PageIdentity>> GetPageByNodeAsync(Guid nodeGUID);
    }
}
