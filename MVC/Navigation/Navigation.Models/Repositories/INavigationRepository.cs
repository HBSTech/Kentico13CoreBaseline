using Navigation.Enums;

namespace Navigation.Repositories
{
    public interface INavigationRepository
    {
        /// <summary>
        /// Get Navigation Items
        /// </summary>
        /// <param name="navPath">The Path to the Navigation items (Usually node alias path for Kentico)</param>
        /// <param name="navTypes">The Navigation Types you wish to grab (usually a list of CategoryNames that the NodeCategories are attached to)</param>
        /// <returns></returns>
        Task<IEnumerable<NavigationItem>> GetNavItemsAsync(Maybe<string> navPath, IEnumerable<string>? navTypes = null);

        /// <summary>
        /// Gets a Secondary Navigation based on the current page's path, the starting level and other settings.
        /// </summary>
        /// <param name="startingPath">The Starting Path of the navigation</param>
        /// <param name="maxLevel">How far down the navigation should be rendered.</param>
        /// <param name="pathType">The selection of the path</param>
        /// <param name="pageTypes">The Page Types (Class Names)</param>
        /// <param name="orderBy">Order by, for Kentico NodeLevel, NodeOrder will follow the tree structure</param>
        /// <param name="whereCondition">The Where Condition, note that if are you selecting multiple page types, you should limit the where condition to only fields shared by them.</param>
        /// <param name="maxLevel">Max nesting level of the pages you wish to select</param>
        /// <param name="topNumber">The Top number of items that you wish to select</param>
        /// <returns>The NavigationItems to render</returns>
        Task<IEnumerable<NavigationItem>> GetSecondaryNavItemsAsync(string startingPath, PathSelectionEnum pathType = PathSelectionEnum.ChildrenOnly, IEnumerable<string>? pageTypes = null, string? orderBy = null, string? whereCondition = null, int? maxLevel = -1, int? topNumber = -1);

        /// <summary>
        /// Gets the path for the ancestor of the given path.
        /// </summary>
        /// <param name="path">The page's path</param>
        /// <param name="levels">How many levels up.  1 = Parent, 2 = Grandparent (if relative), the actual Node Level if not</param>
        /// <param name="levelIsRelative">If the Level number is relative.  If it's not, then 1 = the Root (/), 2 = Secondary Level, 3 = Third level, etc.</param>
        /// <param name="minAbsoluteLevel">If the Level is relative, the minimum level that relative level can be.  This is often helpful to prevent the sub nav from displaying the entire content tree.</param>
        /// <returns>The Ancestor Path</returns>
        Task<string> GetAncestorPathAsync(string path, int levels, bool levelIsRelative = true, int minAbsoluteLevel = 2);

        /// <summary>
        /// Gets the path for the ancestor of the given Node Guid.
        /// </summary>
        /// <param name="nodeGuid">The NodeGuid</param>
        /// <param name="levels">How many levels up.  1 = Parent, 2 = Grandparent (if relative), the actual Node Level if not</param>
        /// <param name="levelIsRelative">If the Level number is relative.  If it's not, then 1 = the Root (/), 2 = Secondary Level, 3 = Third level, etc.</param>
        /// <param name="minAbsoluteLevel">If the Level is relative, the minimum level that relative level can be.  This is often helpful to prevent the sub nav from displaying the entire content tree.</param>
        /// <returns></returns>
        Task<string> GetAncestorPathAsync(Guid nodeGuid, int levels, bool levelIsRelative = true, int minAbsoluteLevel = 2);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeID">The NodeID</param>
        /// <param name="levels">How many levels up.  1 = Parent, 2 = Grandparent (if relative), the actual Node Level if not</param>
        /// <param name="levelIsRelative">If the Level number is relative.  If it's not, then 1 = the Root (/), 2 = Secondary Level, 3 = Third level, etc.</param>
        /// <param name="minAbsoluteLevel">If the Level is relative, the minimum level that relative level can be. 1 = the Root (/)  This is often helpful to prevent the sub nav from displaying the entire content tree.</param>
        /// <returns></returns>
        Task<string> GetAncestorPathAsync(int nodeID, int levels, bool levelIsRelative = true, int minAbsoluteLevel = 2);


       
    }
}