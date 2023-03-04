using CMS.DocumentEngine;
using MVCCaching;

namespace Navigation.KX13.Models
{
    /// <summary>
    /// Helper to build out Hierarchy Parent-child relationships without the limit of the CMS.Document only "Children" property of the TreeNode Class
    /// </summary>
    public class HierarchyTreeNode : ICacheKey
    {
        public TreeNode Page { get; set; }

        public List<HierarchyTreeNode> Children { get; set; } = new List<HierarchyTreeNode>();
        public HierarchyTreeNode(TreeNode Page)
        {
            this.Page = Page;
        }

        public string GetCacheKey()
        {
            return "NodeID_" + Page.NodeID;
        }
    }
}
