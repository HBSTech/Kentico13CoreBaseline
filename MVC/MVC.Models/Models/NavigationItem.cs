using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Models
{
    public class NavigationItem
    {
        public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();

        public string LinkCSSClass { get; set; }
        public string LinkText { get; set; }
        public string LinkHref { get; set; }

        public string LinkTarget { get; set; }
        public string LinkOnClick { get; set; }
        public string LinkAlt { get; set; }
        public bool IsMegaMenu { get; set; } = false;

        public string LinkPagePath { get; set; } = "";
        public Guid LinkPageGUID { get; set; } = Guid.Empty;
        public Guid LinkDocumentGUID { get; set; } = Guid.Empty;
        public int LinkPageID { get; set; } = 0;
        public int LinkDocumentID { get; set; } = 0;

        /// <summary>
        /// Checks if the current page or descendent is current page
        /// </summary>
        /// <param name="PageIdentifier">can pass a string (LinkPagePath/LinkHref match), an Int (LinkPageID match), or a Guid (LinkPageGUID)</param>
        /// <returns></returns>
        public bool IsDescendentCurrentPage(object PageIdentifier)
        {
            return IsCurrentPage(PageIdentifier) || Children.Any(x => x.IsCurrentPage(PageIdentifier) || x.IsDescendentCurrentPage(PageIdentifier));
        }

        /// <summary>
        /// Returns true if the NavItem represents the current page
        /// </summary>
        /// <param name="PageIdentifier">can pass a string (LinkPagePath/LinkHref match), an Int (LinkPageID match), or a Guid (LinkPageGUID)</param>
        public bool IsCurrentPage(object PageIdentifier)
        {
            if (PageIdentifier == null)
            {
                return false;
            }

            switch (PageIdentifier.GetType().FullName)
            {
                case "System.String":
                default:
                    if (LinkPagePath.Equals(Convert.ToString(PageIdentifier), StringComparison.InvariantCultureIgnoreCase) || (!string.IsNullOrWhiteSpace(LinkHref) && LinkHref.Equals(Convert.ToString(PageIdentifier), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return true;
                    }
                    break;
                case "System.Int32":
                case "System.Int16":
                case "System.Int":
                    if (LinkPageID == Convert.ToInt32(PageIdentifier) || LinkDocumentID == Convert.ToInt32(PageIdentifier))
                    {
                        return true;
                    }
                    break;
                case "System.Guid":
                    if (LinkPageGUID == (Guid)PageIdentifier)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public NavigationItem()
        {

        }

    }

}