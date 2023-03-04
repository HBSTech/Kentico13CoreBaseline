using CMS.DocumentEngine;
using CMS.Helpers;

namespace MVCCaching
{
    /// <summary>
    /// Helper Method that allows you to store cache dependencies easily, and also be able to apply them to the IPageRetriever, should NOT inject this but instead declare it since otherwise the cacheKeys will be the same each time.
    /// </summary>
    public static class ICacheDependencyBuilderExtensions
    {
        public static ICacheDependencyBuilder Pages(this ICacheDependencyBuilder builder, IEnumerable<PageIdentity> pages) => builder.Pages(pages.Select(x => x.DocumentID));

        /// <summary>
        /// Any Node Category relationships with Region Categories. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="bySite"></param>
        /// <returns></returns>
        public static ICacheDependencyBuilder AnyRegions(this ICacheDependencyBuilder builder, bool bySite)
        {
            builder.AddKey(bySite ? $"Regionalization|{builder.SiteName()}" : $"Regionalization");

            return builder;
        }

        /// <summary>
        /// Any Node Category relationships with Region Categories by path. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="path">The node alias path of the individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder RegionsByPath(this ICacheDependencyBuilder builder, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return builder;
            }

            return builder.AddKey($"Regionalization|{builder.SiteName()}|{TreePathUtils.EnsureSingleNodePath(path)}");
        }

        /// <summary>
        /// Any Node Category relationships with Region Categories by path. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="path">The node alias path of the individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder RegionsByNode(this ICacheDependencyBuilder builder, int nodeID)
        {
            if (nodeID <= 0)
            {
                return builder;
            }

            return builder.AddKey($"Regionalization|{nodeID}");
        }

        /// <summary>
        /// Any Node Category relationships with Region Categories by path. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="path">The node alias path of the individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder RegionsByNode(this ICacheDependencyBuilder builder, Guid nodeGuid)
        {
            return builder.AddKey($"Regionalization|{nodeGuid}");
        }

        /// <summary>
        /// Any Node Category relationships with Region Categories by path. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="path">The node alias path of the individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder RegionsByNodes(this ICacheDependencyBuilder builder, IEnumerable<int> nodeIDs)
        {
            foreach (var nodeID in nodeIDs)
            {
                RegionsByNode(builder, nodeID);
            }
            return builder;
        }

        public static ICacheDependencyBuilder RegionsByNodes(this ICacheDependencyBuilder builder, IEnumerable<TreeNode> pages) => RegionsByNodes(builder, pages.Select(x => x.NodeID));

        public static ICacheDependencyBuilder RegionsByNodes(this ICacheDependencyBuilder builder, IEnumerable<PageIdentity> pages) => RegionsByNodes(builder, pages.Select(x => x.NodeID));


        /// <summary>
        /// Any Node Category relationships with Region Categories by class name.  Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="className"></param>
        /// <param name="bySite"></param>
        /// <returns></returns>
        public static ICacheDependencyBuilder RegionsByPageType(this ICacheDependencyBuilder builder, string className, bool bySite)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return builder;
            }

            return builder.AddKey(bySite ? $"Regionalization|{builder.SiteName()}|{className}" : $"Regionalization|{className}");
        }


        /// <summary>
        /// Any Node Category relationships with non-region Categories. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="bySite"></param>
        /// <returns></returns>
        public static ICacheDependencyBuilder AnyTreeCategory(this ICacheDependencyBuilder builder, bool bySite)
        {
            builder.AddKey(bySite ? $"TreeCategory|{builder.SiteName()}" : $"TreeCategory");

            return builder;
        }

        /// <summary>
        /// Any Node Category relationships with non-region Categories by path. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="path">The node alias path of the individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByPath(this ICacheDependencyBuilder builder, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return builder;
            }

            return builder.AddKey($"TreeCategory|{builder.SiteName()}|{path}");
        }

        /// <summary>
        /// Any Node Category relationships with non-region Categories by ID. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="nodeID">The node ID path of the individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByNode(this ICacheDependencyBuilder builder, int nodeID)
        {
            if (nodeID <= 0)
            {
                return builder;
            }

            return builder.AddKey($"TreeCategory|{nodeID}");
        }

        /// <summary>
        /// Any Node Category relationships with non-region Categories by ID. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="page">The individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByNode(this ICacheDependencyBuilder builder, TreeNode page) => TreeCategoryByNode(builder, page.NodeID);


        /// <summary>
        /// Any Node Category relationships with non-region Categories by ID. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="page">The individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByNode(this ICacheDependencyBuilder builder, PageIdentity page) => TreeCategoryByNode(builder, page.NodeID);


        /// <summary>
        /// Any Node Category relationships with non-region Categories by Tree node (nodeid). Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="pages">The pages</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByNodes(this ICacheDependencyBuilder builder, IEnumerable<TreeNode> pages) => TreeCategoryByNodes(builder, pages.Select(x => x.NodeID));

        /// <summary>
        /// 
        /// Any Node Category relationships with non-region Categories by Page Identity (nodeid). Triggered on NodeCategoryDependencyTouchModule.cs
        /// <param name="builder"></param>
        /// <param name="pages">The pages</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByNodes(this ICacheDependencyBuilder builder, IEnumerable<PageIdentity> pages) => TreeCategoryByNodes(builder, pages.Select(x => x.NodeID));

        /// <summary>
        /// Any Node Category relationships with non-region Categories by node Ids. Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="path">The node alias path of the individual page</param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByNodes(this ICacheDependencyBuilder builder, IEnumerable<int> nodeIDs)
        {
            foreach (var nodeID in nodeIDs)
            {
                builder.AddKey($"TreeCategory|{nodeID}");
            }
            return builder;
        }

        /// <summary>
        /// Any Node Category relationships with non-region Categories by class name.  Triggered on NodeCategoryDependencyTouchModule.cs
        /// </summary>
        /// <param name="className"></param>
        /// <param name="bySite"></param>
        /// <returns></returns>
        public static ICacheDependencyBuilder TreeCategoryByPageType(this ICacheDependencyBuilder builder, string className, bool bySite)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return builder;
            }

            return builder.AddKey(bySite ? $"TreeCategory|{builder.SiteName()}|{className}" : $"TreeCategory|{className}");
        }

        /// <summary>
        /// Adds Regionalization|NodeID and DocumentID|DocID for multiple pages
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public static ICacheDependencyBuilder DocumentIDAndRegion(this ICacheDependencyBuilder builder, IEnumerable<TreeNode> pages)
        {
            foreach (var page in pages)
            {
                DocumentIDAndRegion(builder, page);
            }
            return builder;
        }

        /// <summary>
        /// Adds Regions and DocumentID for multiple pages
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public static ICacheDependencyBuilder DocumentIDAndRegion(this ICacheDependencyBuilder builder, IEnumerable<PageIdentity> pageIdentities)
        {
            foreach (var pageIdentity in pageIdentities)
            {
                DocumentIDAndRegion(builder, pageIdentity);
            }
            return builder;
        }

        /// <summary>
        /// Adds documentID and Regions for a single page.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ICacheDependencyBuilder DocumentIDAndRegion(this ICacheDependencyBuilder builder, TreeNode page)
        {
            builder.Page(page.DocumentID)
                .RegionsByNode(page.NodeID);
            return builder;
        }

        public static ICacheDependencyBuilder DocumentIDAndRegion(this ICacheDependencyBuilder builder, PageIdentity pageIdentity)
        {
            builder.Page(pageIdentity.DocumentID)
                .RegionsByNode(pageIdentity.NodeID);
            return builder;
        }


        public static ICacheDependencyBuilder Attachment(this ICacheDependencyBuilder builder, IEnumerable<MediaItem> attachments)
        {
            return builder.Attachment(attachments.Select(x => x.MediaGUID));
        }
        public static ICacheDependencyBuilder Attachment(this ICacheDependencyBuilder builder, IEnumerable<Guid> attachmentMediaGuids)
        {
            foreach (var attachmentMediaGuid in attachmentMediaGuids)
            {
                builder.Attachment(attachmentMediaGuid);
            }
            return builder;
        }



        /// <summary>
        /// Dependency for when a page that a navigation item references is updated
        /// </summary>
        /// <returns></returns>
        public static ICacheDependencyBuilder Navigation(this ICacheDependencyBuilder builder, bool bySite)
        {
            return builder.AddKey(bySite ? $"CustomNavigationClearKey|{builder.SiteName()}" : "CustomNavigationClearKey");
        }

        /// <summary>
        /// Dependency for when a page that a navigation item references is updated
        /// </summary>
        /// <returns></returns>
        public static ICacheDependencyBuilder TranslationKeys(this ICacheDependencyBuilder builder)
        {
            return builder
                .AddKey($"{CMS.Localization.ResourceStringInfo.OBJECT_TYPE}|all")
                .AddKey($"{CMS.Localization.ResourceTranslationInfo.OBJECT_TYPE}|all");
        }

        public static ICacheDependencyBuilder TranslationKeys(this ICacheDependencyBuilder builder, IEnumerable<string> translationKeys)
        {
            return builder.AddKeys(translationKeys.Select(x => $"{CMS.Localization.ResourceStringInfo.OBJECT_TYPE}|byname|{x}"));
        }

        public static bool DependenciesNotTouchedSince(this ICacheDependencyBuilder builder, TimeSpan forThisTime)
        {
            var keys = builder.GetKeys().ToArray();
            string cacheKey = string.Join(",", keys).GetHashCode().ToString();

            // If this is the first time calling with these dependencies to check, then this value will indicate that "yes, it's a new check"
            var firstRunCheckTime = CacheHelper.Cache(cs =>
            {
                return DateTime.Now;
            }, new CacheSettings(double.MaxValue, cacheKey));
            var firstRun = DateTime.Now.Subtract(firstRunCheckTime).Seconds < 1;

            var lastTouched = CacheHelper.Cache(cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(keys);
                }
                // For first run, adjust the time to be before the forThisTime, otherwise the current date
                return firstRun ? DateTime.Now.Subtract(forThisTime) : DateTime.Now;
            }, new CacheSettings(double.MaxValue, "DependenciesNotTouched", cacheKey));

            // If the last touched time is less than the forThisTime, then it cleared recently and should wait until the forThisTime Buffer to re-cache
            return (DateTime.Now - lastTouched > forThisTime);
        }

        public static ICacheDependencyBuilder Nodes(this ICacheDependencyBuilder builder, IEnumerable<PageIdentity> pages) => Nodes(builder, pages.Select(x => x.NodeID));

        public static ICacheDependencyBuilder Nodes(this ICacheDependencyBuilder builder, IEnumerable<TreeNode> pages) => Nodes(builder, pages.Select(x => x.NodeID));

        public static ICacheDependencyBuilder Nodes(this ICacheDependencyBuilder builder, IEnumerable<int> nodeIds)
        {
            foreach (var nodeID in nodeIds)
            {
                builder.AddKey($"nodeid|{nodeID}");
            }
            return builder;
        }

    }
}