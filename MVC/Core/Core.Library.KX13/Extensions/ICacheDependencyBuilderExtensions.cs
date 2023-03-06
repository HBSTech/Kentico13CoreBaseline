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