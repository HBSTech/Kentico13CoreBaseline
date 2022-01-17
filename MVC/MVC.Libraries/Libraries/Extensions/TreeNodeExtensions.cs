using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;
using CMS.Helpers;
using CMS.SiteProvider;
using Generic.Models;
using Kentico.Content.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Libraries.Extensions
{
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Converts a TreeNode to the PageIdentity, useful for pages retrieved througH PageBuilder
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static PageIdentity ToPageIdentity(this TreeNode node)
        {
            var pageIdentity = new PageIdentity()
            {
                NodeID = node.NodeID,
                NodeGUID = node.NodeGUID,
                DocumentID = node.DocumentID,
                DocumentGUID = node.DocumentGUID,
                Path = node.NodeAliasPath,
                Alias = node.NodeAlias,
                Name = node.DocumentName,
                NodeLevel = node.NodeLevel
            };

            var relativeAndAbsoluteUrl = CacheHelper.Cache(cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency($"documentid|{pageIdentity.DocumentID}");
                }
                try
                {
                    if (node.NodeSiteID <= 0)
                    {
                        throw new Exception("Need NodeSiteD");
                    }
                    string url = DocumentURLProvider.GetUrl(node);
                    return new Tuple<string, string>(DocumentURLProvider.GetUrl(node), GetAbsoluteUrlOptimized(url, node.NodeSiteID, node.DocumentCulture, true));
                }
                catch (Exception)
                {
                    // Will need to re-query the page, must be missing columns
                }
                if (pageIdentity.DocumentID > 0)
                {
                    // get full page
                    var fullNode = CacheHelper.Cache(cs =>
                    {
                        if (cs.Cached)
                        {
                            cs.CacheDependency = CacheHelper.GetCacheDependency(new string[]
                            {
                                $"documentid{ pageIdentity.DocumentID }"
                            });
                        }
                        return new DocumentQuery()
                            .WhereEquals(nameof(TreeNode.DocumentID), node.DocumentID)
                            .EnsureUrls()
                            .GetEnumerableTypedResult()
                            .FirstOrDefault();
                    }, new CacheSettings(10, "GetDocumentForUrlRetrieval", pageIdentity.DocumentID));

                    string url = DocumentURLProvider.GetUrl(fullNode);
                    return new Tuple<string, string>(url.Replace("~", ""), GetAbsoluteUrlOptimized(url, fullNode.NodeSiteID, fullNode.DocumentCulture, true));
                }
                else
                {
                    return new Tuple<string, string>(string.Empty, string.Empty);
                }
            }, new CacheSettings(10, "GetNodeUrlsForPageIdentity", node.DocumentID));
            pageIdentity.RelativeUrl = relativeAndAbsoluteUrl.Item1;
            pageIdentity.AbsoluteUrl = relativeAndAbsoluteUrl.Item2;
            return pageIdentity;
        }

        /// <summary>
        /// DocumentURLProvider.GetPresentationUrl() does various uncached database calls, this caches that to minimize calls for absolute url
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="siteIdentifier"></param>
        /// <param name="cultureCode"></param>
        /// <param name="ensureUrlFormat"></param>
        /// <returns></returns>
        private static string GetAbsoluteUrlOptimized(string virtualPath, SiteInfoIdentifier siteIdentifier, string cultureCode, bool ensureUrlFormat)
        {
            Uri uri;
            if (siteIdentifier == null)
            {
                throw new ArgumentNullException("siteIdentifier");
            }
            if (URLHelper.IsAbsoluteUrl(virtualPath, out uri))
            {
                return virtualPath;
            }
            string presentationUrl = CacheHelper.Cache(cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(new string[]
                    {
                    $"{SiteInfo.OBJECT_TYPE}|all",
                    $"{SiteDomainAliasInfo.OBJECT_TYPE}|all",
                    });
                }
                return DocumentURLProvider.GetPresentationUrl(siteIdentifier, cultureCode);
            }, new CacheSettings(1440, "GetPresentationUrl", siteIdentifier, cultureCode));

            string str = URLHelper.CombinePath(virtualPath, '/', presentationUrl, null);
            if (!URLHelper.IsAbsoluteUrl(str, out uri))
            {
                throw new InvalidOperationException("Unable to get the page absolute URL since the site presentation URL is not in correct format.");
            }
            if (ensureUrlFormat)
            {
                PageRoutingHelper.EnsureAbsoluteUrlFormat(str, siteIdentifier, out str);
            }
            return str;
        }
    }


}
