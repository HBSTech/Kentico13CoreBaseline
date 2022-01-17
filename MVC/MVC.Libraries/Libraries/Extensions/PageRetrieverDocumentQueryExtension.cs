using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;

namespace Kentico.Content.Web.Mvc
{
    public static class PageRetrieverDocumentQueryExtensions
    {

        /// <summary>
        /// Joins as well as ensures the 2 needed columns are added
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public static DocumentQuery EnsureUrls(this DocumentQuery baseQuery)
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