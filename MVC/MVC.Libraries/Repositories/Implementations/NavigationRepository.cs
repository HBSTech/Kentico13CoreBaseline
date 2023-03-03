using AutoMapper;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using CMS.Helpers;
using Generic.Enums;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Generic.Libraries.Helpers;
using Kentico.Content.Web.Mvc;
using MVCCaching.Base.Core.Interfaces;
using RelationshipsExtended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class NavigationRepository : INavigationRepository
    {
        private readonly IPageRetriever _pageRetriever;
        private readonly ISiteRepository _siteRepository;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public NavigationRepository(IPageRetriever pageRetriever,
            ISiteRepository siteRepository,
            ICacheDependenciesStore cacheDependenciesStore,
            IMapper mapper,
            ILogger logger)
        {
            _pageRetriever = pageRetriever;
            _siteRepository = siteRepository;
            _cacheDependenciesStore = cacheDependenciesStore;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<IEnumerable<NavigationItem>> GetNavItemsAsync(string navPath = null, IEnumerable<string> navTypes = null)
        {
            navTypes ??= Array.Empty<string>();
            var navigationItems = await GetNavigationItemsAsync(navPath, navTypes);

            // Convert to a Hierarchy listing
            var hierarchyItems = await NodeListToHierarchyTreeNodesAsync(navigationItems);

            // Convert to Model
            var items = new List<NavigationItem>();
            foreach (var hierarchyNavTreeNode in hierarchyItems)
            {
                // Call the check to set the Ancestor is current
                var item = await GetTreeNodeToNavigationItemAsync(hierarchyNavTreeNode);
                if (item != null)
                {
                    items.Add(item);
                }

            }
            return await Task.FromResult(items);
        }


        public async Task<IEnumerable<NavigationItem>> GetSecondaryNavItemsAsync(string startingPath, PathSelectionEnum pathType = PathSelectionEnum.ChildrenOnly, IEnumerable<string> pageTypes = null, string orderBy = null, string whereCondition = null, int maxLevel = -1, int topNumber = -1)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);

            pageTypes ??= Array.Empty<string>();
            var hierarchyNodes = new List<HierarchyTreeNode>();
            var nodeIDToHierarchyTreeNode = new Dictionary<int, HierarchyTreeNode>();
            var newNodeList = new List<TreeNode>();
            var pathTypeEnum = pathType switch
            {
                PathSelectionEnum.ParentAndChildren => PathTypeEnum.Section,
                PathSelectionEnum.ParentOnly => PathTypeEnum.Single,
                _ => PathTypeEnum.Children,
            };

            // Add dependency
            builder.PagePath(startingPath, pathTypeEnum);

            var nodes = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
            query.Path(startingPath, pathTypeEnum)
            .If(!string.IsNullOrWhiteSpace(orderBy), query => query.OrderBy(orderBy))
            .If(!string.IsNullOrWhiteSpace(whereCondition), query => query.Where(whereCondition))
            .If(maxLevel > 0, query => query.NestingLevel(maxLevel))
            .If(topNumber > 0, query => query.TopN(topNumber))
            .Columns(new string[] { nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeParentID), nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath) })
            .If(pageTypes.Any(), query => query.Where($"NodeClassID in (select ClassID from CMS_Class where ClassName in ('{string.Join("','", pageTypes.Select(x => SqlHelper.EscapeQuotes(x)))}'))"))
            , cs => cs.Configure(builder, 30, "GetSecondaryNavigationItemsAsync", orderBy, whereCondition, maxLevel, topNumber, (int)pathTypeEnum, startingPath, pageTypes)
            );

            // populate parentNodeIDToTreeNode
            foreach (TreeNode node in nodes)
            {
                nodeIDToHierarchyTreeNode.Add(node.NodeID, new HierarchyTreeNode(node));
                newNodeList.Add(node);
            }

            // Populate the Children of the TypedResults
            foreach (TreeNode node in newNodeList)
            {
                // If no parent exists, add to top level
                if (!nodeIDToHierarchyTreeNode.ContainsKey(node.NodeParentID))
                {
                    hierarchyNodes.Add(nodeIDToHierarchyTreeNode[node.NodeID]);
                }
                else
                {
                    // Otherwise, add to the parent element.
                    nodeIDToHierarchyTreeNode[node.NodeParentID].Children.Add(nodeIDToHierarchyTreeNode[node.NodeID]);
                }
            }

            // Convert to Model
            var items = new List<NavigationItem>();
            foreach (var hierarchyNavTreeNode in hierarchyNodes)
            {
                // Call the check to set the Ancestor is current
                var item = await GetTreeNodeToNavigationItemAsync(hierarchyNavTreeNode);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return await Task.FromResult(items);
        }

        public Task<string> GetAncestorPathAsync(string path, int levels, bool levelIsRelative = true, int minAbsoluteLevel = 2)
        {
            string[] pathParts = TreePathUtils.EnsureSingleNodePath(path).Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (levelIsRelative)
            {
                // Handle minimum absolute level
                if ((pathParts.Length - levels + 1) < minAbsoluteLevel)
                {
                    levels -= minAbsoluteLevel - (pathParts.Length - levels + 1);
                }
                return Task.FromResult(TreePathUtils.GetParentPath(TreePathUtils.EnsureSingleNodePath(path), levels));
            }

            // Since first 'item' in path is actually level 2, reduce level by 1 to match counts
            levels--;
            if (pathParts.Length > levels)
            {
                return Task.FromResult("/" + string.Join("/", pathParts.Take(levels)));
            }
            else
            {
                return Task.FromResult(TreePathUtils.EnsureSingleNodePath(path));
            }
        }

        public async Task<string> GetAncestorPathAsync(Guid nodeGuid, int levels, bool levelIsRelative = true, int minAbsoluteLevel = 2)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore)
                .Node(nodeGuid);

            // Do not need to include in global cache, just a lookup for the path
            var result = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
                    query.WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid)
                    .Columns(nameof(TreeNode.NodeAliasPath))
                    , cs =>
                        cs.Configure(builder, 15, "GetAncestorPathAsync", nodeGuid)
                        );

            return await GetAncestorPathAsync(result.FirstOrDefault()?.NodeAliasPath ?? "/", levels, levelIsRelative);
        }

        public async Task<string> GetAncestorPathAsync(int nodeID, int levels, bool levelIsRelative = true, int minAbsoluteLevel = 2)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore)
                .Node(nodeID);

            // Do not need to include in global cache, just a lookup for the path
            var result = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
                    query.WhereEquals(nameof(TreeNode.NodeID), nodeID)
                    .Columns(nameof(TreeNode.NodeAliasPath))
                    , cs =>
                        cs.Configure(builder, 15, "GetAncestorPathAsync", nodeID)
                        );

            return await GetAncestorPathAsync(result.FirstOrDefault()?.NodeAliasPath ?? "/", levels, levelIsRelative);
        }

        private async Task<NavigationItem> GetTreeNodeToNavigationItemAsync(HierarchyTreeNode hierarchyNavTreeNode)
        {
            var navItem = new NavigationItem();
            if (hierarchyNavTreeNode.Page is Navigation navTreeNode)
            {
                switch ((NavigationTypeEnum)(navTreeNode.NavigationType))
                {
                    case NavigationTypeEnum.Automatic:
                        if (navTreeNode.NavigationPageNodeGuid != Guid.Empty)
                        {
                            var tempNavItem = await GetTreeNodeToNavAsync(navTreeNode.NavigationPageNodeGuid);
                            if (tempNavItem != null)
                            {
                                // Convert to a new navigation item so it's not linked to the cached memory object, specifically the Children List
                                navItem = _mapper.Map<NavigationItem>(tempNavItem);
                            }
                            else
                            {
                                // could not find document
                                _logger.LogInformation("NavigationRepository", "Nav Item Reference not found", $"Could not find page with guid {navTreeNode.NavigationPageNodeGuid} on Navigation item {navTreeNode.NodeAliasPath}");
                                return null;
                            }
                        }
                        else
                        {
                            navItem.LinkText = navTreeNode.NavigationLinkText;
                            navItem.LinkTarget = navTreeNode.NavigationLinkTarget;
                            navItem.LinkHref = navTreeNode.NavigationLinkUrl;
                            navItem.LinkPagePath = navTreeNode.NodeAliasPath;
                            navItem.LinkDocumentID = navTreeNode.DocumentID;
                            navItem.LinkPageID = navTreeNode.NodeID;
                            navItem.LinkPageGUID = navTreeNode.NodeGUID;
                            navItem.LinkDocumentGUID = navTreeNode.DocumentGUID;
                        }
                        break;
                    case NavigationTypeEnum.Manual:
                    default:
                        navItem.LinkText = navTreeNode.NavigationLinkText;
                        navItem.LinkTarget = navTreeNode.NavigationLinkTarget;
                        navItem.LinkHref = navTreeNode.NavigationLinkUrl;
                        navItem.LinkPagePath = navTreeNode.NodeAliasPath;
                        navItem.LinkDocumentID = navTreeNode.DocumentID;
                        navItem.LinkPageID = navTreeNode.NodeID;
                        navItem.LinkPageGUID = navTreeNode.NodeGUID;
                        navItem.LinkDocumentGUID = navTreeNode.DocumentGUID;
                        break;
                }
                // Add additional items
                navItem.IsMegaMenu = navTreeNode.NavigationIsMegaMenu;
                navItem.LinkCSSClass = navTreeNode.NavigationLinkCSS;
                navItem.LinkOnClick = navTreeNode.NavigationLinkOnClick;
                navItem.LinkAlt = navTreeNode.NavigationLinkAlt;
            }
            else
            {
                // Create navigation item from page manually
                navItem = _mapper.Map<NavigationItem>(hierarchyNavTreeNode.Page);
            }

            // Add children
            foreach (var hierarchyChild in hierarchyNavTreeNode.Children)
            {
                var item = await GetTreeNodeToNavigationItemAsync(hierarchyChild);
                if (item != null)
                {
                    navItem.Children.Add(item);
                }
            }
            return navItem;
        }

        private async Task<NavigationItem> GetTreeNodeToNavAsync(Guid linkPageIdentifier)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Node(linkPageIdentifier);

            var document = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.NodeGUID), linkPageIdentifier)
                    .Columns(new string[] {
                        nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath)
                     }),
               cs => cs.Configure(builder, 50, "GetTreeNodeToNavAsync", linkPageIdentifier)
            );

            if (document.Any())
            {
                return _mapper.Map<NavigationItem>(document.FirstOrDefault());
            }
            else
            {
                return null;
            }
        }

        private async Task<IEnumerable<Navigation>> GetNavigationItemsAsync(string navPath, IEnumerable<string> navTypes)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.SitePageType(Navigation.CLASS_NAME);

            var results = await _pageRetriever.RetrieveAsync<Navigation>(query =>
            {
                query.OrderBy(new string[] { nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeOrder) })
                    .Columns(new string[] {
                        nameof(Navigation.NodeParentID), nameof(Navigation.NodeID), nameof(Navigation.NodeGUID), nameof(Navigation.NavigationType), nameof(Navigation.NavigationPageNodeGuid), nameof(Navigation.NavigationLinkText), nameof(Navigation.NavigationLinkTarget), nameof(Navigation.NavigationLinkUrl),
                        nameof(Navigation.NavigationIsMegaMenu), nameof(Navigation.NavigationLinkCSS), nameof(Navigation.NavigationLinkOnClick), nameof(Navigation.NavigationLinkAlt), nameof(Navigation.NodeAliasPath),
                        nameof(Navigation.IsDynamic), nameof(Navigation.Path), nameof(Navigation.PageTypes), nameof(Navigation.OrderBy), nameof(Navigation.WhereCondition), nameof(Navigation.MaxLevel), nameof(Navigation.TopNumber), nameof(Navigation.DocumentID), nameof(Navigation.DocumentGUID)
                   });
                if (!string.IsNullOrWhiteSpace(navPath))
                {
                    query.Path(navPath.Trim('%'), PathTypeEnum.Section);
                }
                if (navTypes?.Any() ?? false)
                {
                    query.TreeCategoryCondition(navTypes);
                }
            }, cs => cs.Configure(builder, 1440, "GetNavigationItemsAsync", navPath, navTypes)
            );

            return results;
        }

        private async Task<List<HierarchyTreeNode>> NodeListToHierarchyTreeNodesAsync(IEnumerable<ITreeNode> nodes)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);

            var hierarchyNodes = new List<HierarchyTreeNode>();

            var nodeIDToHierarchyTreeNode = new Dictionary<int, HierarchyTreeNode>();
            var newNodeList = new List<TreeNode>();

            // populate ParentNodeIDToTreeNode
            foreach (TreeNode node in nodes)
            {
                nodeIDToHierarchyTreeNode.Add(node.NodeID, new HierarchyTreeNode(node));
                newNodeList.Add(node);
                // Get dynamic items below this, and add them to the NodeIDToHierarchyTreeNode

                // Special Handling of Navigation Items
                if (node is Navigation navItem)
                {
                    if (navItem.IsDynamic)
                    {
                        // Build NodeIDtoHierarchyTreeNode from the underneath
                        string path = "/" + navItem.Path.Trim('%').Trim('/');
                        builder.Clear()
                            .PagePath(path);

                        var dynamicNodes = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
                        query.Path(path, PathTypeEnum.Children)
                        .If(!string.IsNullOrWhiteSpace(navItem.OrderBy), query => query.OrderBy(navItem.OrderBy))
                        .If(!string.IsNullOrWhiteSpace(navItem.WhereCondition), query => query.Where(navItem.WhereCondition))
                        .If(navItem.TryGetValue("MaxLevel", out var maxLevel), quey => query.NestingLevel(ValidationHelper.GetInteger(maxLevel, -1)))
                        .If(navItem.TryGetValue("TopNumber", out var topN), query => query.TopN(ValidationHelper.GetInteger(topN, -1)))
                        .Columns(new string[] { nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeParentID), nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath) })
                        .If((navItem.PageTypes?.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).Any(), query => query.Where($"NodeClassID in (select ClassID from CMS_Class where ClassName in ('{string.Join("','", navItem.PageTypes.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => SqlHelper.EscapeQuotes(x)))}'))"))
                        , cs => cs.Configure(builder, 60, "NodeListToHierarchyTreeNodesAsync", navItem.NodeGUID)
                        );

                        if (dynamicNodes.Any())
                        {
                            int MinNodeLevel = dynamicNodes.Min(x => x.NodeLevel);
                            foreach (TreeNode dynamicNode in dynamicNodes)
                            {
                                // Change the "Parent" to be this navigation node
                                if (dynamicNode.NodeLevel == MinNodeLevel)
                                {
                                    dynamicNode.NodeParentID = navItem.NodeID;
                                }
                                nodeIDToHierarchyTreeNode.Add(dynamicNode.NodeID, new HierarchyTreeNode(dynamicNode));
                                newNodeList.Add(dynamicNode);
                            }
                        }
                    }
                }

            }

            // Populate the Children of the TypedResults
            foreach (TreeNode node in newNodeList)
            {
                // If no parent exists, add to top level
                if (!nodeIDToHierarchyTreeNode.ContainsKey(node.NodeParentID))
                {
                    hierarchyNodes.Add(nodeIDToHierarchyTreeNode[node.NodeID]);
                }
                else
                {
                    // Otherwise, add to the parent element.
                    nodeIDToHierarchyTreeNode[node.NodeParentID].Children.Add(nodeIDToHierarchyTreeNode[node.NodeID]);
                }
            }
            return hierarchyNodes;
        }
    }
}