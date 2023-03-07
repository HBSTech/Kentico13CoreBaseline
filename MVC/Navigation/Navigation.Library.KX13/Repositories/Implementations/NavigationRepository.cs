using CMS.Base;
using CMS.DataEngine;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Kentico.Content.Web.Mvc;
using Microsoft.Extensions.Localization;
using System.Data;
using CMS.DocumentEngine.Types;
using MVCCaching;
using NavigationPageType = CMS.DocumentEngine.Types.Generic.Navigation;
using RelationshipsExtended;

namespace Navigation.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class NavigationRepository : INavigationRepository
    {
        private readonly IPageRetriever _pageRetriever;
        private readonly ISiteRepository _siteRepository;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ILogger _logger;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IMediaRepository _mediaRepository;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;
        private readonly IProgressiveCache _progressiveCache;

        public NavigationRepository(IPageRetriever pageRetriever,
            ISiteRepository siteRepository,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ILogger logger,
            IStringLocalizer<SharedResources> stringLocalizer,
            IMediaRepository mediaRepository,
            ICacheRepositoryContext cacheRepositoryContext,
            IProgressiveCache progressiveCache)
        {
            _pageRetriever = pageRetriever;
            _siteRepository = siteRepository;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _mediaRepository = mediaRepository;
            _cacheRepositoryContext = cacheRepositoryContext;
            _progressiveCache = progressiveCache;
        }


        public async Task<IEnumerable<NavigationItem>> GetNavItemsAsync(Maybe<string> navPath, IEnumerable<string>? navTypes = null)
        {

            var navigationItems = await GetNavigationItemsAsync(navPath, navTypes ?? Array.Empty<string>());

            // Convert to a Hierarchy listing
            var hierarchyItems = await NodeListToHierarchyTreeNodesAsync(navigationItems);

            // Convert to Model
            var items = new List<NavigationItem>();
            foreach (var hierarchyNavTreeNode in hierarchyItems)
            {
                // Call the check to set the Ancestor is current
                var item = await GetTreeNodeToNavigationItemAsync(hierarchyNavTreeNode);
                if (item.IsSuccess)
                {
                    items.Add(item.Value);
                }

            }
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<NavigationItem>> GetSecondaryNavItemsAsync(string startingPath, PathSelectionEnum pathType = PathSelectionEnum.ChildrenOnly, IEnumerable<string>? pageTypes = null, string? orderBy = null, string? whereCondition = null, int? maxLevel = -1, int? topNumber = -1)
        {
            return await GetSecondaryNavItemsAsync(startingPath, pathType, pageTypes != null ? pageTypes.WithEmptyAsNone() : Maybe.None, orderBy.AsNullOrWhitespaceMaybe(), whereCondition.AsNullOrWhitespaceMaybe(), maxLevel != null && maxLevel.Value != -1 ? Maybe.From(maxLevel.Value) : Maybe<int>.None, topNumber != null && topNumber.Value != -1 ? Maybe.From(topNumber.Value) : Maybe<int>.None);
        }
        public async Task<IEnumerable<NavigationItem>> GetSecondaryNavItemsAsync(string startingPath, PathSelectionEnum pathType = PathSelectionEnum.ChildrenOnly) => await GetSecondaryNavItemsAsync(startingPath, pathType, Maybe.None, Maybe.None, Maybe.None, Maybe.None, Maybe.None);
        public async Task<IEnumerable<NavigationItem>> GetSecondaryNavItemsAsync(string startingPath, PathSelectionEnum pathType, Maybe<IEnumerable<string>> pageTypes, Maybe<string> orderBy, Maybe<string> whereCondition, Maybe<int> maxLevel, Maybe<int> topNumber)
        {
            var builder = _cacheDependencyBuilderFactory.Create();

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

            var nodes = await _pageRetriever.RetrieveAsync<TreeNode>(query => query
                .Path(startingPath, pathTypeEnum)
                .If(orderBy.TryGetValueNonEmpty(out var orderByVal), quer => query.OrderBy(orderByVal))
                .If(whereCondition.TryGetValueNonEmpty(out var whereConditionVal), query => query.Where(whereConditionVal))
                .If(maxLevel.HasValue, query => query.NestingLevel(maxLevel.Value))
                .If(topNumber.HasValue, query => query.TopN(topNumber.Value))
                .Columns(new string[] { nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeParentID), nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath) })
                .If(pageTypes.TryGetValueNonEmpty(out var pageTypesVal), query => query.Where($"NodeClassID in (select ClassID from CMS_Class where ClassName in ('{string.Join("','", pageTypesVal.Select(x => SqlHelper.EscapeQuotes(x)))}')"))
                , cacheSettings => cacheSettings.Configure(builder, CacheMinuteTypes.Long.ToDouble(), "GetSecondaryNavigationItemsAsync", startingPath, pageTypes.GetValueOrDefault(Array.Empty<string>()), orderBy.GetValueOrDefault(string.Empty), whereCondition.GetValueOrDefault(string.Empty), maxLevel.GetValueOrDefault(0), topNumber.GetValueOrDefault(0))
            );

            builder.Pages(nodes);

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
                if (item.IsSuccess)
                {
                    items.Add(item.Value);
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
            // Do not need to include in global cache, just a lookup for the path
            var result = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
                    query.WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid)
                    .Columns(nameof(TreeNode.NodeAliasPath))
                    , cacheSettings =>
                        cacheSettings
                        .Dependencies((items, csbuilder) => csbuilder.Custom($"nodeguid|{_siteRepository.CurrentSiteName()}|{nodeGuid}"))
                        .Key($"GetAncestorPathAsync|{nodeGuid}")
                        .Expiration(TimeSpan.FromMinutes(15))
                        );

            return await GetAncestorPathAsync(result.FirstOrDefault()?.NodeAliasPath ?? "/", levels, levelIsRelative);
        }

        public async Task<string> GetAncestorPathAsync(int nodeID, int levels, bool levelIsRelative = true, int minAbsoluteLevel = 2)
        {
            // Do not need to include in global cache, just a lookup for the path
            var result = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
                    query.WhereEquals(nameof(TreeNode.NodeID), nodeID)
                    .Columns(nameof(TreeNode.NodeAliasPath))
                    , cacheSettings =>
                        cacheSettings
                        .Dependencies((items, csbuilder) => csbuilder.Custom($"nodeguid|{_siteRepository.CurrentSiteName()}|{nodeID}"))
                        .Key($"GetAncestorPathAsync|{nodeID}")
                        .Expiration(TimeSpan.FromMinutes(15))
                        );

            return await GetAncestorPathAsync(result.FirstOrDefault()?.NodeAliasPath ?? "/", levels, levelIsRelative);
        }

        private async Task<Result<NavigationItem>> GetTreeNodeToNavigationItemAsync(HierarchyTreeNode hierarchyNavTreeNode)
        {
            var nodeGuidToClass = await NodeGuidToClassNameAsync();
            var navItem = new NavigationItem(hierarchyNavTreeNode.Page.DocumentName);

            if (hierarchyNavTreeNode.Page is NavigationPageType navTreeNode)
            {
                switch ((NavigationTypeEnum)(navTreeNode.NavigationType))
                {
                    case NavigationTypeEnum.Automatic:
                        if (navTreeNode.NavigationPageNodeGuid != Guid.Empty && nodeGuidToClass.GetValueOrMaybe(navTreeNode.NavigationPageNodeGuid).TryGetValue(out var className))
                        {

                            var tempNavItem = await GetNavigationFromNodeInfo(navTreeNode.NavigationPageNodeGuid, className);
                            if (tempNavItem.TryGetValue(out var tempNavItemVal))
                            {
                                // Convert to a new navigation item so it's not linked to the cached memory object, specifically the Children List
                                navItem = CloneNavItem(tempNavItemVal);
                            }
                            else
                            {
                                // could not find document
                                _logger.LogInformation("NavigationRepository", "Nav Item Reference not found", $"Could not find page with guid {navTreeNode.NavigationPageNodeGuid} on Navigation item {navTreeNode.NodeAliasPath}");
                                return Result.Failure<NavigationItem>($"Could not find page with guid {navTreeNode.NavigationPageNodeGuid} on Navigation item {navTreeNode.NodeAliasPath}");
                            }
                        }
                        else
                        {
                            navItem.LinkText = navTreeNode.NavigationLinkText.StartsWith("navigation.", StringComparison.OrdinalIgnoreCase) ? _stringLocalizer.GetString(navTreeNode.NavigationLinkText) : navTreeNode.NavigationLinkText;
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
                        // get link text

                        // Check for localization on the optional localized name or the required NavigationLinkText
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
                // Treat as an automatic nav type
                var page = hierarchyNavTreeNode.Page;

                var tempNavItem = await GetNavigationFromNodeInfo(page.NodeGUID, page.ClassName);
                if (tempNavItem.TryGetValue(out var tempNavItemVal))
                {
                    // Convert to a new navigation item so it's not linked to the cached memory object, specifically the Children List
                    navItem = CloneNavItem(tempNavItemVal);
                }
                else
                {
                    // could not find document
                    _logger.LogInformation("NavigationRepository", "Non Nav Item not found", $"Could not find page with guid {page.NodeGUID}: {tempNavItem.Error}");
                    return Result.Failure<NavigationItem>($"Could not find page with guid {page.NodeGUID}");
                }
            }

            // Add children
            foreach (var hierarchyChild in hierarchyNavTreeNode.Children)
            {
                var item = await GetTreeNodeToNavigationItemAsync(hierarchyChild);
                if (item.IsSuccess)
                {
                    navItem.Children.Add(item.Value);
                }
            }
            // Call the nav levels
            navItem.InitializeNavLevels();
            return navItem;
        }

        private NavigationItem CloneNavItem(NavigationItem navItem)
        {
            return new NavigationItem(navItem.LinkText)
            {
                NavLevel = navItem.NavLevel,
                Children = new List<NavigationItem>(),
                LinkCSSClass = navItem.LinkCSSClass,
                LinkHref = navItem.LinkHref,
                LinkTarget = navItem.LinkTarget,
                LinkOnClick = navItem.LinkOnClick,
                LinkAlt = navItem.LinkAlt,
                LinkPagePath = navItem.LinkPagePath,
                LinkPageGUID = navItem.LinkPageGUID,
                LinkDocumentGUID = navItem.LinkDocumentGUID,
                LinkPageID = navItem.LinkPageID,
                LinkDocumentID = navItem.LinkDocumentID
            };
        }


        private async Task<Dictionary<Guid, string>> NodeGuidToClassNameAsync()
        {
            return await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(new string[] { "cms.tree|all", "cms.node|all" });
                }
                return (await XperienceCommunityConnectionHelper.ExecuteQueryAsync("select NodeGuid, ClassName from CMS_Tree inner join CMS_Class on ClassID = NodeClassID", new QueryDataParameters(), QueryTypeEnum.SQLQuery))
                .Tables[0].Rows.Cast<DataRow>().Select(x => new Tuple<Guid, string>((Guid)x["NodeGuid"], (string)x["ClassName"])).GroupBy(x => x.Item1).ToDictionary(key => key.Key, value => value.First().Item2);
            }, new CacheSettings(CacheMinuteTypes.Long.ToDouble(), "GetNodeToClassName"));
        }


        /// <summary>
        /// Retrieves each document individually so can have expanded columns, caches adn converts to a Navigation Item.
        /// </summary>
        /// <param name="nodeGuid"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private async Task<Result<NavigationItem>> GetNavigationFromNodeInfo(Guid nodeGuid, string className)
        {
            var builder = _cacheDependencyBuilderFactory.Create()
               .Node(nodeGuid);

            // Get any type now, this includes Tabs, no special columns needed
            var treeDocument = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid)
                    .Columns(new string[] {
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.ClassName),
                        nameof(TreeNode.DocumentCulture),
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.NodeAliasPath)
                     }),
               cacheSettings => cacheSettings.Configure(builder, CacheMinuteTypes.Medium.ToDouble(), "GetTreeNodeToNavAsyncTreeNode", nodeGuid)
            );

            if (treeDocument.FirstOrMaybe().TryGetValue(out var treeDoc))
            {
                var prioritizeLocalizedValue = treeDoc.DocumentCulture.Equals("en-US", StringComparison.OrdinalIgnoreCase);

                // Normal Tree node to Navigation
                var navItem = new NavigationItem(treeDoc.DocumentName)
                {
                    LinkHref = DocumentURLProvider.GetUrl(treeDoc).RemoveTildeFromFirstSpot(),
                    LinkPagePath = treeDoc.NodeAliasPath,
                    LinkPageGUID = treeDoc.NodeGUID,
                    LinkPageID = treeDoc.NodeID,
                    LinkDocumentID = treeDoc.DocumentID,
                    LinkDocumentGUID = treeDoc.DocumentGUID
                };

                return navItem;
            }
            return Result.Failure<NavigationItem>($"Could not retrieve page {nodeGuid}");

        }

        private async Task<IEnumerable<NavigationPageType>> GetNavigationItemsAsync(Maybe<string> navPath, IEnumerable<string> navTypes)
        {
            var builder = _cacheDependencyBuilderFactory.Create();

            var results = await _pageRetriever.RetrieveAsync<NavigationPageType>(query => query
                    .OrderBy(new string[] { nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeOrder) })
                    .Columns(new string[] {
                        nameof(NavigationPageType.NodeParentID), nameof(NavigationPageType.NodeID), nameof(NavigationPageType.NodeGUID), nameof(NavigationPageType.NavigationType), nameof(NavigationPageType.NavigationPageNodeGuid), nameof(NavigationPageType.NavigationLinkText), nameof(NavigationPageType.NavigationLinkTarget), nameof(NavigationPageType.NavigationLinkUrl),
                        nameof(NavigationPageType.NavigationLinkCSS), nameof(NavigationPageType.NavigationLinkOnClick), nameof(NavigationPageType.NavigationLinkAlt), nameof(NavigationPageType.NodeAliasPath),
                        nameof(NavigationPageType.IsDynamic), nameof(NavigationPageType.Path), nameof(NavigationPageType.PageTypes), nameof(NavigationPageType.OrderBy), nameof(NavigationPageType.WhereCondition), nameof(NavigationPageType.MaxLevel), nameof(NavigationPageType.TopNumber), nameof(NavigationPageType.DocumentID), nameof(NavigationPageType.DocumentGUID)
                   })
                   .If(navPath.TryGetValueNonEmpty(out var navPathVal), query => query.Path(navPathVal.Trim('%'), PathTypeEnum.Section))
                   .If(navTypes.Any(), query => query.TreeCategoryCondition(navTypes))
                   ,
                   cacheSettings => cacheSettings.Configure(builder, CacheMinuteTypes.VeryLong.ToDouble(), "GetNavigationItemsAsync", navPath.GetValueOrDefault(string.Empty), navTypes.GetValueOrDefault(Array.Empty<string>()))
            );

            return results;
        }

        private async Task<List<HierarchyTreeNode>> NodeListToHierarchyTreeNodesAsync(IEnumerable<ITreeNode> nodes)
        {
            var builder = _cacheDependencyBuilderFactory.Create();

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
                if (node is NavigationPageType navItem)
                {
                    if (navItem.IsDynamic)
                    {
                        // Build NodeIDtoHierarchyTreeNode from the underneath
                        string path = "/" + navItem.Path.Trim('%').Trim('/');
                        builder = _cacheDependencyBuilderFactory.Create()
                            .PagePath(path, PathTypeEnum.Children);

                        var dynamicNodes = await _pageRetriever.RetrieveAsync<TreeNode>(query =>
                        {
                            query.Path(path, PathTypeEnum.Children);
                            if (!string.IsNullOrWhiteSpace(navItem.OrderBy))
                            {
                                query.OrderBy(navItem.OrderBy);
                            }
                            if (!string.IsNullOrWhiteSpace(navItem.WhereCondition))
                            {
                                query.Where(navItem.WhereCondition);
                            }
                            if (navItem.TryGetValue("MaxLevel", out var maxLevel))
                            {
                                query.NestingLevel(ValidationHelper.GetInteger(maxLevel, -1));
                            }
                            if (navItem.TryGetValue("TopNumber", out var topN))
                            {
                                query.TopN(ValidationHelper.GetInteger(topN, -1));
                            }
                            query.Columns(new string[] { nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeParentID), nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath) });
                            var pageTypes = navItem.PageTypes.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (pageTypes.Any())
                            {
                                query.Where($"NodeClassID in (select ClassID from CMS_Class where ClassName in ('{string.Join("','", pageTypes)}')");
                            }
                        }, cacheSettings => cacheSettings.Configure(builder, CacheMinuteTypes.Medium.ToDouble(), "NodeListToHierarchyTreeNodesTreeNodeAsync", navItem.NodeGUID));

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