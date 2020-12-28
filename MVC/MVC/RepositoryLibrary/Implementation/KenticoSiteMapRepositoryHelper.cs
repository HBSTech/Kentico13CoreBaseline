using CMS.DocumentEngine;
using CMS.Helpers;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoSiteMapRepositoryHelper : IKenticoSiteMapRepositoryHelper
    {
        private IRepoContext _repoContext;
        private IServiceProvider _serviceProvider;

        public KenticoSiteMapRepositoryHelper(IRepoContext repoContext, IServiceProvider serviceProvider)
        {
            _repoContext = repoContext;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the NodeLevel that current NodeAliasPath is at.
        /// </summary>
        /// <param name="Path">The Path to search</param>
        /// <param name="SiteName">The Site Name</param>
        /// <returns>It's NodeLevel</returns>
        [CacheDependency("node|{1}|{0}")]
        public int GetNodeLevel(string Path, string SiteName)
        {
            var Node = DocumentHelper.GetDocuments()
                    .Path(Path, PathTypeEnum.Single)
                    .OnSite(SiteName)
                    .Columns("NodeLevel")
                    .FirstOrDefault();
            return (Node != null ? Node.NodeLevel : 0);
        }

        /// <summary>
        /// Given the Path, Classname, and Options, gets the Urls from the UrlColumnName property of the Options
        /// </summary>
        /// <param name="Path">The parent Nodealiaspath</param>
        /// <param name="ClassName">The Class Name to query</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">The Options</param>
        /// <returns>The relative Urls to add to the sitemap</returns>
        [CacheDependency("nodes|{2}|{1}|all")]
        [CacheDependency("node|{2}|{0}")]
        public IEnumerable<string> GetRelativeUrls(string Path, string ClassName, string SiteName, SiteMapOptions Options)
        {
            var DocumentQuery = GetDocumentQuery(Path, Options, ClassName);
            return DocumentQuery.TypedResult.Select(x => x.GetStringValue(Options.UrlColumnName, DocumentURLProvider.GetUrl(x)));
        }

        /// <summary>
        /// Converts the ITreeNode into an SitemapNode with absolute Url.
        /// </summary>
        /// <param name="Node">The Node</param>
        /// <param name="SiteName">The SiteName</param>
        /// <returns>The SitemapNode</returns>
        public SitemapNode ConvertNodeToSiteMapUrl(TreeNode Node, string SiteName)
        {
            string Url = URLHelper.GetAbsoluteUrl(DocumentURLProvider.GetUrl(Node), RequestContext.CurrentDomain);
            SitemapNode SiteMapItem = new SitemapNode(Url)
            {
                LastModificationDate = Node.DocumentModifiedWhen
            };
            return SiteMapItem;
        }

        /// <summary>
        /// Gets the SitemapNodes for any Class given the path and the options.
        /// </summary>
        /// <param name="Path">The parent Nodealiaspath</param>
        /// <param name="ClassName">The Class Name to query</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">The Options</param>
        /// <returns>The SitemapNodes</returns>
        [CacheDependency("node|{1}|{0}")]
        [CacheDependency("node|{1}|{0}|ChildNodes")]
        public IEnumerable<SitemapNode> GetSiteMapUrlSetForAllClass(string Path, string SiteName, SiteMapOptions Options)
        {
            var DocumentQuery = GetDocumentQuery(Path, Options);
            return DocumentQuery.TypedResult.Select(x => ConvertNodeToSiteMapUrl(x, SiteName));
        }

        /// <summary>
        /// Gets the SitemapNodes for the given Class and Path
        /// </summary>
        /// <param name="Path">The parent Nodealiaspath</param>
        /// <param name="ClassName">The Class Name to query</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">The Options</param>
        /// <returns>The SitemapNodes</returns>
        [CacheDependency("nodes|{2}|{1}|all")]
        [CacheDependency("node|{2}|{0}")]
        public IEnumerable<SitemapNode> GetSiteMapUrlSetForClass(string Path, string ClassName, string SiteName, SiteMapOptions Options)
        {
            var DocumentQuery = GetDocumentQuery(Path, Options, ClassName);
            return DocumentQuery.TypedResult.Select(x => ConvertNodeToSiteMapUrl(x, SiteName));
        }

        /// <summary>
        /// Helper method to set up the initial DocumentQuery
        /// </summary>
        /// <param name="Path">The Path (ex /) of the parent you wish to scan for.  It will include the path plus any children.</param>
        /// <param name="Options">The SiteMapoptions </param>
        /// <param name="ClassName">The Classname, if you are searching a specific one.</param>
        /// <returns>The Base Query</returns>
        [DoNotCache]
        public DocumentQuery GetDocumentQuery(string Path, SiteMapOptions Options, string ClassName = null)
        {
            var DocumentQuery = (string.IsNullOrWhiteSpace(ClassName) ? new DocumentQuery() : new DocumentQuery(ClassName));
            DocumentQuery
                    .Path(Path, PathTypeEnum.Section)
                    .Culture(DataHelper.GetNotEmpty(Options.CultureCode, _repoContext.CurrentCulture()))
                    .OnSite(Options.SiteName)
                    .Published(Options.SelectOnlyPublished);
            if (Options.CheckDocumentPermissions.HasValue)
            {
                DocumentQuery.CheckPermissions(Options.CheckDocumentPermissions.Value);
            }
            if (Options.CombineWithDefaultCulture.HasValue)
            {
                DocumentQuery.CombineWithDefaultCulture(Options.CombineWithDefaultCulture.Value);
            }
            if (Options.MaxRelativeLevel > -1)
            {
                // Get the nesting level of the give path
                IKenticoSiteMapRepositoryHelper _CachableSelfHelper = (IKenticoSiteMapRepositoryHelper)_serviceProvider.GetService(typeof(IKenticoNavigationRepositoryHelper));
                DocumentQuery.NestingLevel(Options.MaxRelativeLevel + _CachableSelfHelper.GetNodeLevel(Path, Options.SiteName));
            }

            if (!string.IsNullOrWhiteSpace(Options.WhereCondition))
            {
                DocumentQuery.Where(Options.WhereCondition);
            }
            return DocumentQuery;
        }
    }
}