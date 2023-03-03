namespace Core.Repositories
{
    public interface IMediaRepository
    {
        /// <summary>
        /// Gets the Attachment Url, if using this method the page must include the DocumentID column if using Kentico
        /// </summary>
        /// <param name="documentID">The Page's document id, if not provided will use the current page context</param>
        /// <returns></returns>
        Task<IEnumerable<MediaItem>> GetPageAttachmentsAsync(int? documentID = null);

        /// <summary>
        /// Gets the Attachment Url, if using this method the page must include the DocumentID column if using Kentico
        /// </summary>
        /// <param name="attachmentGuid">The Attachment Guid</param>
        /// <param name="documentID">The page's document id, if not provided will use the current page context</param>
        /// <returns></returns>
        Task<Result<MediaItem>> GetPageAttachmentAsync(Guid attachmentGuid, int? documentID = null);

        /// <summary>
        /// Gets the Attachment item
        /// </summary>
        /// <param name="attachmentGuid">The Attachment Guid</param>
        /// <returns></returns>
        Task<Result<MediaItem>> GetAttachmentItemAsync(Guid attachmentGuid);

        /// <summary>
        /// Gets the IEnumerable of Attachment Items
        /// </summary>
        /// <param name="attachmentGuids">IEnumerable of Attachment Guids</param>
        /// <returns></returns>
        Task<IEnumerable<MediaItem>> GetAttachmentsAsync(IEnumerable<Guid> attachmentGuids);

        /// <summary>
        /// Gets the Media File Item
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <returns></returns>
        Task<Result<MediaItem>> GetMediaItemAsync(Guid fileGuid);

        /// <summary>
        /// Gets the Media Items by the given Library Code Name
        /// </summary>
        /// <param name="libraryName"></param>
        /// <returns></returns>
        Task<IEnumerable<MediaItem>> GetMediaItemsByLibraryAsync(string libraryName);

        /// <summary>
        /// The path of the files, either the full path (including the library 
        /// </summary>
        /// <param name="path">The path within the media library folder</param>
        /// <param name="libraryName">Optional Library name</param>
        /// <returns></returns>
        Task<IEnumerable<MediaItem>> GetMediaItemsByPathAsync(string path, string? libraryName = null);

        /// <summary>
        /// Gets the sitename of the media or attachment by Guid so it can be appended, used primarily in the middleware to update getattachment or getmedia links automatically
        /// </summary>
        /// <returns></returns>
        Task<Result<string>> GetMediaAttachmentSiteNameAsync(Guid mediaOrAttachmentGuid);

    }
}
