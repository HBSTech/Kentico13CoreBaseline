using MVCCaching;
using System.Collections.Generic;

namespace Generic.Models
{
    public class SiteMapOptions : ICacheKey
    {
        /// <summary>
        /// Determines the path to start from
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Determines which page types are displayed. Specify the page types as a list of code names separated by semicolons (;). If empty, only CMS.MenuItem pages are loaded by default.
        /// </summary>
        public IEnumerable<string> ClassNames { get; set; }

        /// <summary>
        /// Specifies whether the default language version of pages is used as a replacement for pages that are not translated into the currently selected language. If you select the 'Use site settings' option, the web part loads the value from 'Settings -> Content -> Combine with default culture'.
        /// </summary>
        public bool? CombineWithDefaultCulture { get; set; }

        /// <summary>
        /// Indicates which culture version of the specified pages should be used.
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        /// Specifies the maximum number of content tree sub-levels from which the content is to be loaded. This number is relative, i.e. it is counted from the beginning of the path specified for the content of the web part. Entering -1 causes all sub-levels to be included.
        /// </summary>
        public int MaxRelativeLevel { get; set; } = -1;

        /// <summary>
        /// Specifies whether the web part only loads published pages.
        /// </summary>
        public bool SelectOnlyPublished { get; set; } = true;

        /// <summary>
        /// Defines the website (specified by its code name) from which to load the content. If left empty, the current site is used.
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// Sets the value of the WHERE clause in the SELECT statement used to retrieve the content.
        /// </summary>
        public string WhereCondition { get; set; }

        /// <summary>
        /// Indicates whether the web part checks the permissions of the users viewing the content. If enabled, the web part only loads pages for which the user has the 'Read' permission.
        /// </summary>
        public bool? CheckDocumentPermissions { get; set; }

        /// <summary>
        /// Sets the name of the cache key used for the content of the web part. If not specified, this name is generated automatically based on the site, page path, Web part control ID and current user. A cache key can be shared between multiple web parts with the same content on different pages in order to avoid keeping redundant data in the memory.
        /// </summary>
        public string CacheItemName { get; set; }

        /// <summary>
        /// The Column name to get the relative Url field from, useful for Navigation page types
        /// </summary>
        public string UrlColumnName { get; set; }

        public string GetCacheKey()
        {
            return (!string.IsNullOrWhiteSpace(CacheItemName) ? $"{Path}|{(ClassNames != null ? string.Join(",",ClassNames) : "")}|{CombineWithDefaultCulture}|{CultureCode}|{MaxRelativeLevel}|{SelectOnlyPublished}|{SiteName}|{WhereCondition}|{CheckDocumentPermissions}" : CacheItemName);
        }
    }
}