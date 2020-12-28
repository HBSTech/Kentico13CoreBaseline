using CMS.DocumentEngine;
using MVCCaching;
using System;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IMediaRepository : IRepository
    {
        /// <summary>
        /// Gets the Attachment Url, if using this method the page must include the DocumentID column if using Kentico
        /// </summary>
        /// <param name="Page">The Page</param>
        /// <param name="ImageGuid">The Attachment Guid</param>
        /// <returns></returns>
        string GetAttachmentImage(TreeNode Page, Guid ImageGuid);
        Task<string> GetAttachmentImageAsync(TreeNode Page, Guid ImageGuid);

        /// <summary>
        /// Gets the Attachment Url
        /// </summary>
        /// <param name="ImageGuid">The Attachment Guid</param>
        /// <returns></returns>
        string GetAttachmentImage(Guid ImageGuid);
        Task<string> GetAttachmentImageAsync(Guid ImageGuid);

        /// <summary>
        /// Gets the Media File Url
        /// </summary>
        /// <param name="FileGuid">The media file Guid</param>
        /// <returns>The Media File Url</returns>
        string GetMediaFileUrl(Guid FileGuid);
        Task<string> GetMediaFileUrlAsync(Guid FileGuid);
    }
}