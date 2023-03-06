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
        /// <param name="identity">The Node Identity (you can use string/int/guid .ToNodeIdentity() extension)</param>
        /// <returns>The Page Identity</returns>
        Task<Result<PageIdentity>> GetPageAsync(NodeIdentity identity);

        /// <summary>
        /// Gets the PageIdentity information based on the given identifier
        /// </summary>
        /// <param name="identity">The Document Identity (you can use string/int/guid .ToDocumentIdentity() extension)</param>
        /// <returns>The Page Identity </returns>
        Task<Result<PageIdentity>> GetPageAsync(DocumentIdentity identity);

        }
}
