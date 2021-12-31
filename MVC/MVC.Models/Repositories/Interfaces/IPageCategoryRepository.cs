﻿using Generic.Models;
using MVCCaching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IPageCategoryRepository : IRepository
    {
        /// <summary>
        /// Gets the page's categories (node categories)
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns>The Categories</returns>
        Task<IEnumerable<CategoryItem>> GetCategoriesByNodeAsync(int nodeID);

        /// <summary>
        /// Gets the pages categories (node categories)
        /// </summary>
        /// <param name="nodeIDs">the nodes</param>
        /// <returns>The Categories of all the nodes</returns>
        Task<IEnumerable<CategoryItem>> GetCategoriesByNodesAsync(IEnumerable<int> nodeIDs);

        /// <summary>
        /// Gets the page's categories (node categories)
        /// </summary>
        /// <param name="path">The page's path</param>
        /// <returns>The Categories</returns>
        Task<IEnumerable<CategoryItem>> GetCategoryItemsByPathAsync(string path);
    }
}
