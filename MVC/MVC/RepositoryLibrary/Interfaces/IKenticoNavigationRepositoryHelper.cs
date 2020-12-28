using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.DocumentEngine.Types.Generic;
using Generic.Models;
using MVCCaching;

namespace Generic.Repositories.Helpers.Interfaces
{
    public interface IKenticoNavigationRepositoryHelper : IRepository
    {
        /// <summary>
        /// Gets the TreeNode based on the NodeGuid and converts it to a NavigationItem
        /// </summary>
        /// <param name="linkPageIdentifier">The NodeGuid</param>
        /// <returns>The NavigationItem</returns>
        NavigationItem GetTreeNodeToNav(Guid linkPageIdentifier);

        Task<NavigationItem> GetTreeNodeToNavAsync(Guid linkPageIdentifier);


        /// <summary>
        /// Takes teh HierarchyTreeNode and converts it and it's children into a NavigationItem with it's children set
        /// </summary>
        /// <param name="HierarchyNavTreeNode">The Hierarchy Tree Node</param>
        /// <returns>The NavigationItem</returns>
        NavigationItem GetTreeNodeToNavigationItem(HierarchyTreeNode HierarchyNavTreeNode);
        Task<NavigationItem> GetTreeNodeToNavigationItemAsync(HierarchyTreeNode HierarchyNavTreeNode);


        /// <summary>
        /// Gets the Navigation objects given the Path and categories
        /// </summary>
        /// <param name="navPath">The Path for the Navigation items</param>
        /// <param name="navTypes">Code names for categories, can use to show some nav items conditionally.</param>
        /// <returns></returns>
        IEnumerable<Navigation> GetNavigationItems(string navPath, string[] navTypes = null);
        Task<IEnumerable<Navigation>> GetNavigationItemsAsync(string navPath, string[] navTypes = null);
    }
}