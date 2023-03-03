namespace Core.Repositories
{
    public interface IMetaDataRepository
    {
        /// <summary>
        /// Gets the Meta Data based on the current page
        /// </summary>
        /// <param name="Thumbnail">Optional Thumbnail override</param>
        /// <returns>The Page Meta Data object</returns>
        Task<Result<PageMetaData>> GetMetaDataAsync(string? thumbnail = null);
        /// <summary>
        /// Gets the Meta Data based on the given page
        /// </summary>
        /// <param name="Thumbnail">Optional Thumbnail override</param>
        /// <returns>The Page Meta Data object</returns>
        Task<Result<PageMetaData>> GetMetaDataAsync(int documentId, string? thumbnail = null);
        /// <summary>
        /// Gets the Meta Data based on the given page
        /// </summary>
        /// <param name="Thumbnail">Optional Thumbnail override</param>
        /// <returns>The Page Meta Data object</returns>
        Task<Result<PageMetaData>> GetMetaDataAsync(Guid documentGuid, string? thumbnail = null);
    }
}
