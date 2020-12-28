using CMS.Base;
using Generic.Enums;
using Generic.Models;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface INavigationRepository : IRepository
    {
        /// <summary>
        /// Get Navigation Items
        /// </summary>
        /// <param name="NavPath">The Path to the Navigation items (Usually node alias path for Kentico)</param>
        /// <param name="NavTypes">The Navigation Types you wish to grab (usually a list of CategoryNames that the NodeCategories are attached to)</param>
        /// <returns></returns>
        IEnumerable<NavigationItem> GetNavItems(string NavPath = null, string[] NavTypes = null);
        Task<IEnumerable<NavigationItem>> GetNavItemsAsync(string NavPath = null, string[] NavTypes = null);


        /// <summary>
        /// Gets a Secondary Navigation based on the current page's path, the starting level and other settings.
        /// </summary>
        /// <param name="StartingPath">The Starting Path of the navigation</param>
        /// <param name="MaxLevel">How far down the navigation should be rendered.</param>
        /// <param name="PathType">The </param>
        /// <param name="PageTypes">The Page Types (Class Names)</param>
        /// <param name="OrderBy">Order by, for Kentico NodeLevel, NodeOrder will follow the tree structure</param>
        /// <param name="WhereCondition">The Where Condition, note that if are you selecting multiple page types, you should limit the where condition to only fields shared by them.</param>
        /// <param name="MaxLevel">Max nesting level of the pages you wish to select</param>
        /// <param name="TopNumber">The Top number of items that you wish to select</param>
        /// <returns>The NavigationItems to render</returns>
        IEnumerable<NavigationItem> GetSecondaryNavItems(string StartingPath, PathSelectionEnum PathType = PathSelectionEnum.ChildrenOnly, string[] PageTypes = null, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1);
        Task<IEnumerable<NavigationItem>> GetSecondaryNavItemsAsync(string StartingPath, PathSelectionEnum PathType = PathSelectionEnum.ChildrenOnly, string[] PageTypes = null, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1);

        /// <summary>
        /// Gets the path for the ancestor of the given path.
        /// </summary>
        /// <param name="Path">The page's path</param>
        /// <param name="Levels">How many levels up.  1 = Parent, 2 = Grandparent (if relative), the actual Node Level if not</param>
        /// <param name="LevelIsRelative">If the Level number is relative.  If it's not, then 1 = the Root (/), 2 = Secondary Level, 3 = Third level, etc.</param>
        /// <param name="MinAbsoluteLevel">If the Level is relative, the minimum level that relative level can be.  This is often helpful to prevent the sub nav from displaying the entire content tree.</param>
        /// <returns>The Ancestor Path</returns>
        string GetAncestorPath(string Path, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2);
        Task<string> GetAncestorPathAsync(string Path, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2);

        /// <summary>
        /// Gets the path for the ancestor of the given Node Guid.
        /// </summary>
        /// <param name="NodeGuid">The NodeGuid</param>
        /// <param name="Levels">How many levels up.  1 = Parent, 2 = Grandparent (if relative), the actual Node Level if not</param>
        /// <param name="LevelIsRelative">If the Level number is relative.  If it's not, then 1 = the Root (/), 2 = Secondary Level, 3 = Third level, etc.</param>
        /// <param name="MinAbsoluteLevel">If the Level is relative, the minimum level that relative level can be.  This is often helpful to prevent the sub nav from displaying the entire content tree.</param>
        /// <returns></returns>
        string GetAncestorPath(Guid NodeGuid, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2);
        Task<string> GetAncestorPathAsync(Guid NodeGuid, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NodeID">The NodeID</param>
        /// <param name="Levels">How many levels up.  1 = Parent, 2 = Grandparent (if relative), the actual Node Level if not</param>
        /// <param name="LevelIsRelative">If the Level number is relative.  If it's not, then 1 = the Root (/), 2 = Secondary Level, 3 = Third level, etc.</param>
        /// <param name="MinAbsoluteLevel">If the Level is relative, the minimum level that relative level can be. 1 = the Root (/)  This is often helpful to prevent the sub nav from displaying the entire content tree.</param>
        /// <returns></returns>
        string GetAncestorPath(int NodeID, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2);
        Task<string> GetAncestorPathAsync(int NodeID, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2);


        /// <summary>
        /// Converts the given Nodes into a HierarchyTreeNode list
        /// </summary>
        /// <param name="Nodes">The Nodes to convert</param>
        List<HierarchyTreeNode> NodeListToHierarchyTreeNodes(IEnumerable<ITreeNode> Nodes);
        Task<List<HierarchyTreeNode>> NodeListToHierarchyTreeNodesAsync(IEnumerable<ITreeNode> Nodes);


    }
}