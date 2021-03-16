using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using Generic.Enums;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RelationshipsExtended;
namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoNavigationRepositoryHelper : IKenticoNavigationRepositoryHelper
    {
        private IRepoContext _repoContext;
        private IMapper _Mapper;
        private IServiceProvider _serviceProvider;

        public KenticoNavigationRepositoryHelper(IRepoContext repoContext, IMapper Mapper, IServiceProvider serviceProvider)
        {
            _repoContext = repoContext;
            _Mapper = Mapper;
            _serviceProvider = serviceProvider;
        }

        public NavigationItem GetTreeNodeToNavigationItem(HierarchyTreeNode HierarchyNavTreeNode)
        {
            return GetTreeNodeToNavigationItemAsync(HierarchyNavTreeNode).Result;
        }

        public async Task<NavigationItem> GetTreeNodeToNavigationItemAsync(HierarchyTreeNode HierarchyNavTreeNode)
        {
            IKenticoNavigationRepositoryHelper _CachableSelfHelper = (IKenticoNavigationRepositoryHelper)_serviceProvider.GetService(typeof(IKenticoNavigationRepositoryHelper));
            NavigationItem NavItem = new NavigationItem();
            if (HierarchyNavTreeNode.Page is Navigation)
            {
                Navigation NavTreeNode = (Navigation)HierarchyNavTreeNode.Page;
                switch ((NavigationTypeEnum)(NavTreeNode.NavigationType))
                {
                    case NavigationTypeEnum.Automatic:
                        if (NavTreeNode.NavigationPageNodeGuid != Guid.Empty)
                        {
                            var TempNavItem = _CachableSelfHelper.GetTreeNodeToNav(NavTreeNode.NavigationPageNodeGuid);
                            // Convert to a new navigation item so it's not linked to the cached memory object, specifically the Children List
                            NavItem = _Mapper.Map<NavigationItem>(TempNavItem);
                        }
                        else
                        {
                            NavItem.LinkText = NavTreeNode.NavigationLinkText;
                            NavItem.LinkTarget = NavTreeNode.NavigationLinkTarget;
                            NavItem.LinkHref = NavTreeNode.NavigationLinkUrl;
                            NavItem.LinkPagePath = NavTreeNode.NodeAliasPath;
                            NavItem.LinkDocumentID = NavTreeNode.DocumentID;
                            NavItem.LinkPageID = NavTreeNode.NodeID;
                            NavItem.LinkPageGuid = NavTreeNode.NodeGUID;
                            NavItem.LinkDocumentGuid = NavTreeNode.DocumentGUID;
                        }
                        break;
                    case NavigationTypeEnum.Manual:
                    default:
                        NavItem.LinkText = NavTreeNode.NavigationLinkText;
                        NavItem.LinkTarget = NavTreeNode.NavigationLinkTarget;
                        NavItem.LinkHref = NavTreeNode.NavigationLinkUrl;
                        NavItem.LinkPagePath = NavTreeNode.NodeAliasPath;
                        NavItem.LinkDocumentID = NavTreeNode.DocumentID;
                        NavItem.LinkPageID = NavTreeNode.NodeID;
                        NavItem.LinkPageGuid = NavTreeNode.NodeGUID;
                        NavItem.LinkDocumentGuid = NavTreeNode.DocumentGUID;
                        break;
                }
                // Add additional items
                NavItem.IsMegaMenu = NavTreeNode.NavigationIsMegaMenu;
                NavItem.LinkCSSClass = NavTreeNode.NavigationLinkCSS;
                NavItem.LinkOnClick = NavTreeNode.NavigationLinkOnClick;
                NavItem.LinkAlt = NavTreeNode.NavigationLinkAlt;
            }
            else
            {
                // Create navigation item from page manually
                NavItem = _Mapper.Map<NavigationItem>(HierarchyNavTreeNode.Page);
            }

            // Add children
            foreach (var HierarchyChild in HierarchyNavTreeNode.Children)
            {
                NavItem.Children.Add(GetTreeNodeToNavigationItem(HierarchyChild));
            }
            return NavItem;
        }

        [CacheDependency("nodeguid|##SITENAME##|{0}")]
        public NavigationItem GetTreeNodeToNav(Guid linkPageIdentifier)
        {
            return GetTreeNodeToNavAsync(linkPageIdentifier).Result;
        }


        [CacheDependency("nodeguid|##SITENAME##|{0}")]
        public async Task<NavigationItem> GetTreeNodeToNavAsync(Guid linkPageIdentifier)
        {
            var DocQuery = DocumentHelper.GetDocuments()
                        .WhereEquals("NodeGuid", linkPageIdentifier)
                        .Culture(_repoContext.CurrentCulture())
                        .CombineWithDefaultCulture()
                        .CombineWithAnyCulture()
                        .OnCurrentSite()
                        .Columns(nameof(TreeNode.DocumentName), nameof(TreeNode.ClassName), nameof(TreeNode.DocumentCulture), nameof(TreeNode.NodeID), nameof(TreeNode.DocumentID), nameof(TreeNode.DocumentGUID), nameof(TreeNode.NodeGUID), nameof(TreeNode.NodeAliasPath));
            if (!_repoContext.PreviewEnabled())
            {
                DocQuery.Published();
            }
            else
            {
                DocQuery.LatestVersion(true);
                DocQuery.Published(false);
            }
            var TreeItem = DocQuery.FirstOrDefault();
            if (TreeItem == null)
            {
                return null;
            }

            return _Mapper.Map<NavigationItem>(TreeItem);
        }

        [CacheDependency("nodes|##SITENAME##|generic.navigation|all")]
        // This cache key is manually triggered upon Navigation Category changes or Page updates to pages the navigation points to
        //[CacheDependency("CustomNavigationClearKey")]
        public IEnumerable<Navigation> GetNavigationItems(string NavPath, string[] NavTypes)
        {
            return GetNavigationItemsAsync(NavPath, NavTypes).Result;
        }

        [CacheDependency("nodes|##SITENAME##|generic.navigation|all")]
        // This cache key is manually triggered upon Navigation Category changes or Page updates to pages the navigation points to
        //[CacheDependency("CustomNavigationClearKey")]
        public async Task<IEnumerable<Navigation>> GetNavigationItemsAsync(string NavPath, string[] NavTypes)
        {
            var NavigationItems = DocumentHelper.GetDocuments<Navigation>()
           .OrderBy("NodeLevel, NodeOrder")
           .Culture(_repoContext.CurrentCulture())
           .CombineWithDefaultCulture()
           .CombineWithAnyCulture()
           .Published(!_repoContext.PreviewEnabled())
           .LatestVersion(_repoContext.PreviewEnabled())
           .Columns(new string[] {
                nameof(Navigation.NodeParentID), nameof(Navigation.NodeID), nameof(Navigation.NodeGUID), nameof(Navigation.NavigationType), nameof(Navigation.NavigationPageNodeGuid), nameof(Navigation.NavigationLinkText), nameof(Navigation.NavigationLinkTarget), nameof(Navigation.NavigationLinkUrl),
                nameof(Navigation.NavigationIsMegaMenu), nameof(Navigation.NavigationLinkCSS), nameof(Navigation.NavigationLinkOnClick), nameof(Navigation.NavigationLinkAlt), nameof(Navigation.NodeAliasPath),
                nameof(Navigation.IsDynamic), nameof(Navigation.Path), nameof(Navigation.PageTypes), nameof(Navigation.OrderBy), nameof(Navigation.WhereCondition), nameof(Navigation.MaxLevel), nameof(Navigation.TopNumber), nameof(Navigation.DocumentID), nameof(Navigation.DocumentGUID)
           });

            if (!string.IsNullOrWhiteSpace(NavPath))
            {
                NavigationItems.Path(NavPath.Trim('%'), PathTypeEnum.Section);
            }

            // Handle Nav Type with Categories found
            if (NavTypes != null && NavTypes.Length > 0)
            {
                NavigationItems.TreeCategoryCondition(NavTypes);
            }
            return NavigationItems.TypedResult;
        }
    }
}