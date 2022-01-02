using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;
using System.Linq;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Extension to make sure the required columns are included...bug submitted to Kentico to have the base "WithPageUrlPaths()" do this.
    /// </summary>
    public static class IPageRetrieverDocumentQueryExtensions
    {
        /// <summary>
        /// Joins as well as ensures the 2 needed columns are added
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public static DocumentQuery EnsureUrls(this DocumentQuery baseQuery)
        {
            if(baseQuery.SelectColumnsList?.Any() ?? false) { 
                baseQuery.AddColumns(nameof(TreeNode.NodeID), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeSiteID));
            }
            baseQuery.WithPageUrlPaths();
            return baseQuery;
        }

        /// <summary>
        /// Joins as well as ensures the 2 needed columns are added
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public static DocumentQuery<TDocument> EnsureUrls<TDocument>(this DocumentQuery<TDocument> baseQuery) where TDocument : TreeNode, new()
        {
            baseQuery.AddColumns(nameof(TreeNode.NodeID), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeSiteID));
            baseQuery.WithPageUrlPaths();
            return baseQuery;
        }
        /// <summary>
        /// Joins as well as ensures the 2 needed columns are added
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public static MultiDocumentQuery EnsureUrls(this MultiDocumentQuery baseQuery)
        {
            baseQuery.AddColumns(nameof(TreeNode.NodeID), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeSiteID));
            baseQuery.WithPageUrlPaths();
            return baseQuery;
        }
    }
}