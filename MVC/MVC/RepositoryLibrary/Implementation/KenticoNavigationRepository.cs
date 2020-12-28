using CMS.Base;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using Generic.Enums;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using Generic.Repositories.Interfaces;
using MVCCaching;
//using MVCCaching.Kentico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    public class KenticoNavigationRepository : INavigationRepository
    {
        private IKenticoNavigationRepositoryHelper _helper;
        private IGeneralDocumentRepository _GeneralDocumentRepo;

        public KenticoNavigationRepository(IGeneralDocumentRepository GeneralDocumentRepository, IKenticoNavigationRepositoryHelper Helper)
        {
            _GeneralDocumentRepo = GeneralDocumentRepository;
            _helper = Helper;
        }

        public List<HierarchyTreeNode> NodeListToHierarchyTreeNodes(IEnumerable<ITreeNode> Nodes)
        {
            return NodeListToHierarchyTreeNodesAsync(Nodes).Result;
        }

        /// <summary>
        /// Converts a list of TreeNodes into a HierarchyTreeNode, putting any children in the Children Property
        /// </summary>
        /// <param name="Nodes">The list of TreeNodes</param>
        /// <returns>List of HierarchyTreeNodes</returns>
        public async Task<List<HierarchyTreeNode>> NodeListToHierarchyTreeNodesAsync(IEnumerable<ITreeNode> Nodes)
        {
            List<HierarchyTreeNode> HierarchyNodes = new List<HierarchyTreeNode>();

            Dictionary<int, HierarchyTreeNode> NodeIDToHierarchyTreeNode = new Dictionary<int, HierarchyTreeNode>();
            List<TreeNode> NewNodeList = new List<TreeNode>();

            // populate ParentNodeIDToTreeNode
            foreach (TreeNode Node in Nodes)
            {
                NodeIDToHierarchyTreeNode.Add(Node.NodeID, new HierarchyTreeNode(Node));
                NewNodeList.Add(Node);

                // Special Handling of Navigation Items
                if (Node is Navigation)
                {
                    // Get dynamic items below this, and add them to the NodeIDToHierarchyTreeNode
                    Navigation NavItem = (Navigation)Node;

                    if (NavItem.IsDynamic)
                    {
                        // Build NodeIDtoHierarchyTreeNode from the underneath
                        var DynamicNodes = _GeneralDocumentRepo.GetDocumentsByPath(
                            "/" + NavItem.Path.Trim('%').Trim('/'),
                            PathSelectionEnum.ChildrenOnly,
                            NavItem.OrderBy,
                            NavItem.WhereCondition,
                            NavItem.GetValue("MaxLevel") == null ? -1 : NavItem.MaxLevel,
                            NavItem.GetValue("TopNumber") == null ? -1 : NavItem.TopNumber,
                            new string[] { nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeParentID), nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath) },
                            NavItem.PageTypes.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            )
                            .Select(x => (TreeNode)x);

                        if (DynamicNodes.Count() > 0)
                        {
                            int MinNodeLevel = DynamicNodes.Min(x => x.NodeLevel);
                            foreach (TreeNode DynamicNode in DynamicNodes)
                            {
                                // Change the "Parent" to be this navigation node
                                if (DynamicNode.NodeLevel == MinNodeLevel)
                                {
                                    DynamicNode.NodeParentID = NavItem.NodeID;
                                }
                                NodeIDToHierarchyTreeNode.Add(DynamicNode.NodeID, new HierarchyTreeNode(DynamicNode));
                                NewNodeList.Add(DynamicNode);
                            }
                        }
                    }
                }

            }

            // Populate the Children of the TypedResults
            foreach (TreeNode Node in NewNodeList)
            {
                // If no parent exists, add to top level
                if (!NodeIDToHierarchyTreeNode.ContainsKey(Node.NodeParentID))
                {
                    HierarchyNodes.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
                else
                {
                    // Otherwise, add to the parent element.
                    NodeIDToHierarchyTreeNode[Node.NodeParentID].Children.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
            }
            return HierarchyNodes;
        }

        [DoNotCache]
        public IEnumerable<NavigationItem> GetNavItems(string NavPath, string[] NavTypes = null)
        {
            return GetNavItemsAsync(NavPath, NavTypes).Result;
        }

        [DoNotCache]
        public async Task<IEnumerable<NavigationItem>> GetNavItemsAsync(string NavPath, string[] NavTypes = null)
        {
            var NavigationItems = _helper.GetNavigationItems(NavPath, NavTypes);

            // Convert to a Hierarchy listing
            var HierarchyItems = NodeListToHierarchyTreeNodes(NavigationItems);

            // Convert to Model
            List<NavigationItem> Items = new List<NavigationItem>();
            foreach (var HierarchyNavTreeNode in HierarchyItems)
            {
                // Call the check to set the Ancestor is current
                Items.Add(_helper.GetTreeNodeToNavigationItem(HierarchyNavTreeNode));
            }
            return Items;
        }

        [DoNotCache]
        public IEnumerable<NavigationItem> GetSecondaryNavItems(string StartingPath, PathSelectionEnum PathType = PathSelectionEnum.ChildrenOnly, string[] PageTypes = null, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1)
        {
            return GetSecondaryNavItemsAsync(StartingPath, PathType, PageTypes, OrderBy, WhereCondition, MaxLevel, TopNumber).Result;
        }

        [DoNotCache]
        public async Task<IEnumerable<NavigationItem>> GetSecondaryNavItemsAsync(string StartingPath, PathSelectionEnum PathType = PathSelectionEnum.ChildrenOnly, string[] PageTypes = null, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1)
        {
            List<HierarchyTreeNode> HierarchyNodes = new List<HierarchyTreeNode>();

            Dictionary<int, HierarchyTreeNode> NodeIDToHierarchyTreeNode = new Dictionary<int, HierarchyTreeNode>();
            List<TreeNode> NewNodeList = new List<TreeNode>();
            IEnumerable<TreeNode> Nodes = _GeneralDocumentRepo.GetDocumentsByPath(
                            TreePathUtils.EnsureSingleNodePath(StartingPath),
                            PathType,
                            OrderBy,
                            WhereCondition,
                            MaxLevel,
                            TopNumber,
                            new string[] { nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeParentID), nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath) },
                            PageTypes
                            )
                            .Select(x => (TreeNode)x);

            // populate ParentNodeIDToTreeNode
            foreach (TreeNode Node in Nodes)
            {
                NodeIDToHierarchyTreeNode.Add(Node.NodeID, new HierarchyTreeNode(Node));
                NewNodeList.Add(Node);

            }

            // Populate the Children of the TypedResults
            foreach (TreeNode Node in NewNodeList)
            {
                // If no parent exists, add to top level
                if (!NodeIDToHierarchyTreeNode.ContainsKey(Node.NodeParentID))
                {
                    HierarchyNodes.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
                else
                {
                    // Otherwise, add to the parent element.
                    NodeIDToHierarchyTreeNode[Node.NodeParentID].Children.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
            }

            // Convert to Model
            List<NavigationItem> Items = new List<NavigationItem>();
            foreach (var HierarchyNavTreeNode in HierarchyNodes)
            {
                // Call the check to set the Ancestor is current
                Items.Add(_helper.GetTreeNodeToNavigationItem(HierarchyNavTreeNode));
            }
            return Items;
        }

        [DoNotCache]
        public string GetAncestorPath(string Path, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            return GetAncestorPathAsync(Path, Levels, LevelIsRelative, MinAbsoluteLevel).Result;
        }

        [DoNotCache]
        public async Task<string> GetAncestorPathAsync(string Path, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            string[] PathParts = TreePathUtils.EnsureSingleNodePath(Path).Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (LevelIsRelative)
            {
                // Handle minimum absolute level
                if ((PathParts.Length - Levels + 1) < MinAbsoluteLevel)
                {
                    Levels -= MinAbsoluteLevel - (PathParts.Length - Levels + 1);
                }
                return TreePathUtils.GetParentPath(TreePathUtils.EnsureSingleNodePath(Path), Levels);
            }

            // Since first 'item' in path is actually level 2, reduce level by 1 to match counts
            Levels--;
            if (PathParts.Count() > Levels)
            {
                return "/" + string.Join("/", PathParts.Take(Levels));
            }
            else
            {
                return TreePathUtils.EnsureSingleNodePath(Path);
            }
        }

        [DoNotCache]
        public string GetAncestorPath(Guid NodeGuid, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            return GetAncestorPathAsync(NodeGuid, Levels, LevelIsRelative, MinAbsoluteLevel).Result;
        }

        [DoNotCache]
        public async Task<string> GetAncestorPathAsync(Guid NodeGuid, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            return GetAncestorPath(_GeneralDocumentRepo.GetDocumentByNodeGuid(NodeGuid, null, Columns: new string[] { "NodeAliasPath" }).NodeAliasPath, Levels, LevelIsRelative);
        }

        [DoNotCache]
        public string GetAncestorPath(int NodeID, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            return GetAncestorPathAsync(NodeID, Levels, LevelIsRelative, MinAbsoluteLevel).Result;
        }

        [DoNotCache]
        public async Task<string> GetAncestorPathAsync(int NodeID, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            return GetAncestorPath(_GeneralDocumentRepo.GetDocumentByNodeID(NodeID, null, Columns: new string[] { "NodeAliasPath" }).NodeAliasPath, Levels, LevelIsRelative);
        }
    }
}