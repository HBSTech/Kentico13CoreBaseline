using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;
using CMS.Helpers;
using CMS.SiteProvider;

namespace Core.Extensions
{
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Converts a TreeNode to the PageIdentity, useful for pages retrieved througH PageBuilder
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static PageIdentity<TPage> ToPageIdentity<TPage>(this TPage node) where TPage : TreeNode
        {
            // Get Urls
            var relativeAndAbsoluteUrl = CacheHelper.Cache(cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency($"documentid|{node.DocumentID}");
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
                if (node.DocumentID > 0)
                {
                    // get full page
                    var fullNode = CacheHelper.Cache(cs =>
                    {
                        if (cs.Cached)
                        {
                            cs.CacheDependency = CacheHelper.GetCacheDependency(new string[]
                            {
                                $"documentid{ node.DocumentID }"
                            });
                        }
                        return new DocumentQuery()
                            .WhereEquals(nameof(TreeNode.DocumentID), node.DocumentID)
                            .WithPageUrlPaths()
                            .GetEnumerableTypedResult()
                            .FirstOrMaybe();
                    }, new CacheSettings(10, "GetDocumentForUrlRetrieval", node.DocumentID));

                    if (fullNode.TryGetValue(out var fullNodeVal))
                    {
                        string url = DocumentURLProvider.GetUrl(fullNodeVal);
                        return new Tuple<string, string>(url.RemoveTildeFromFirstSpot(), GetAbsoluteUrlOptimized(url, fullNodeVal.NodeSiteID, fullNodeVal.DocumentCulture, true));
                    }
                }

                return new Tuple<string, string>(string.Empty, string.Empty);

            }, new CacheSettings(10, "GetNodeUrlsForPageIdentity", node.DocumentID));

            var pageIdentity = new PageIdentity<TPage>(
                node.DocumentName,
                node.NodeAlias,
                node.NodeID,
                node.NodeGUID,
                node.DocumentID,
                node.DocumentGUID,
                node.NodeAliasPath,
                node.DocumentCulture,
                relativeAndAbsoluteUrl.Item1,
                relativeAndAbsoluteUrl.Item2,
                node.NodeLevel,
                node.NodeSiteID,
                node);

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
