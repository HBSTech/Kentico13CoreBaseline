using CMS.DocumentEngine;
using Generic.Models;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.RepositoryLibrary.Interfaces
{
    public interface IMetaDataRepository : IRepository
    {
        /// <summary>
        /// Gets the Meta Data based on the current page
        /// </summary>
        /// <param name="Thumbnail">Optional Thumbnail override</param>
        /// <returns>The Page Meta Data object</returns>
        Task<PageMetaData> GetMetaDataAsync(string thumbnail = "");
        /// <summary>
        /// Gets the Meta Data based on the given page
        /// </summary>
        /// <param name="Thumbnail">Optional Thumbnail override</param>
        /// <returns>The Page Meta Data object</returns>
        Task<PageMetaData> GetMetaDataAsync(TreeNode node, string thumbnail = "");
        /// <summary>
        /// Gets the Meta Data based on the given page
        /// </summary>
        /// <param name="Thumbnail">Optional Thumbnail override</param>
        /// <returns>The Page Meta Data object</returns>
        Task<PageMetaData> GetMetaDataAsync(int documentId, string thumbnail = "");
        /// <summary>
        /// Gets the Meta Data based on the given page
        /// </summary>
        /// <param name="Thumbnail">Optional Thumbnail override</param>
        /// <returns>The Page Meta Data object</returns>
        Task<PageMetaData> GetMetaDataAsync(Guid documentGuid, string thumbnail = "");
    }
}
