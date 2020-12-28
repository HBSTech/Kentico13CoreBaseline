using System.Collections.Generic;
using CMS.DocumentEngine;
using Generic.Models;
using MVCCaching;

namespace Generic.Repositories.Helpers.Interfaces
{
    /// <summary>
    /// Helper class to the KenticoSiteMapRepository
    /// </summary>
    public interface IKenticoSiteMapRepositoryHelper : IRepository
    {
        /// <summary>
        /// Gets the NodeLevel for the given path
        /// </summary>
        /// <param name="Path">The Node Alias Path</param>
        /// <param name="SiteName">The SiteName</param>
        /// <returns>The Node Level</returns>
        int GetNodeLevel(string Path, string SiteName);

        /// <summary>
        /// Gets the Relative Urls for the nodes requested
        /// </summary>
        /// <param name="Path">The Path query (ex "/%")</param>
        /// <param name="ClassName">The Class Name you wish to get</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">SiteMap Options for retrieval</param>
        /// <returns>A list of relative Urls</returns>
        IEnumerable<string> GetRelativeUrls(string Path, string ClassName, string SiteName, SiteMapOptions Options);

        /// <summary>
        /// Gets the SiteMapNodes for the given Path across all class names
        /// </summary>
        /// <param name="Path">The Path query (ex "/%")</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">SiteMap Options for retrieval</param>
        /// <returns>The SiteMapUrls</returns>
        IEnumerable<SitemapNode> GetSiteMapUrlSetForAllClass(string Path, string SiteName, SiteMapOptions Options);

        /// <summary>
        /// Gets the SitemapNodes for the given nodes requested
        /// </summary>
        /// <param name="Path">The Path query (ex "/%")</param>
        /// <param name="ClassName">The Class Name of the nodes you wish to get</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">SiteMap Options for retrieval</param>
        /// <returns>The SiteMapUrls</returns>
        IEnumerable<SitemapNode> GetSiteMapUrlSetForClass(string Path, string ClassName, string SiteName, SiteMapOptions Options);

        /// <summary>
        /// Gets the base DocumentQuery
        /// </summary>
        /// <param name="Path">The Path</param>
        /// <param name="Options"> The SiteMap Options</param>
        /// <param name="ClassName">The Class Name</param>
        /// <returns>The Base DocumentQuery</returns>
        DocumentQuery GetDocumentQuery(string Path, SiteMapOptions Options, string ClassName = null);

        /// <summary>
        /// Converts the TreeNode to a SitemapNode
        /// </summary>
        /// <param name="Node">The Tree Node</param>
        /// <param name="SiteName">The Sitename</param>
        /// <returns>The Sitemap Node</returns>
        SitemapNode ConvertNodeToSiteMapUrl(TreeNode Node, string SiteName);
    }
}