using Generic.Models;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IMediaRepository : IRepository
    {
        /// <summary>
        /// Gets the Attachment Url, if using this method the page must include the DocumentID column if using Kentico
        /// </summary>
        /// <param name="documentID">The Page's document id, if not provided will use the current page context</param>
        /// <returns></returns>
        Task<IEnumerable<AttachmentItem>> GetPageAttachmentsAsync(int documentID = 0);

        /// <summary>
        /// Gets the Attachment Url, if using this method the page must include the DocumentID column if using Kentico
        /// </summary>
        /// <param name="attachmentGuid">The Attachment Guid</param>
        /// <param name="documentID">The page's document id, if not provided will use the current page context</param>
        /// <returns></returns>
        Task<AttachmentItem> GetPageAttachmentAsync(Guid attachmentGuid, int documentID = 0);

        /// <summary>
        /// Gets the Attachment item
        /// </summary>
        /// <param name="imageGuid">The Attachment Guid</param>
        /// <returns></returns>
        Task<AttachmentItem> GetAttachmentItemAsync(Guid attachmentGuid);

        /// <summary>
        /// Gets the Media File Item
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <returns></returns>
        Task<MediaItem> GetMediaItemAsync(Guid fileGuid);

    }
}