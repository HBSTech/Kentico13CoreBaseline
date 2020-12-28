using CMS.DocumentEngine;
using Generic.Repositories.Interfaces;
using Generic.ViewModels;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Components
{
    [ViewComponent(Name = "SecondaryNavigation")]
    public class SecondaryNavigationViewComponent : ViewComponent
    {
        public SecondaryNavigationViewComponent(INavigationRepository navigationRepository, IPageDataContextRetriever pageDataContextRetriever, IUrlResolver urlResolver)
        {
            NavigationRepository = navigationRepository;
            PageDataContextRetriever = pageDataContextRetriever;
            UrlResolver = urlResolver;
        }

        public INavigationRepository NavigationRepository { get; }
        public IPageDataContextRetriever PageDataContextRetriever { get; }
        public IUrlResolver UrlResolver { get; }

        public async Task<IViewComponentResult> InvokeAsync(string NodeAliasPath, int Level, bool LevelIsRelative = true, int MinimumAbsoluteLevel = 2, string CurrentPage = null)
        {
            // Use ViewBag to set current Path
            if (!string.IsNullOrWhiteSpace(CurrentPage))
            {
                if (CurrentPage == "/Home")
                {
                    // Special case for Home since the Link is just "/"
                    CurrentPage = "/";
                }
            }
            else
            {
                if (PageDataContextRetriever.TryRetrieve(out IPageDataContext<TreeNode> Context))
                {
                    CurrentPage = UrlResolver.ResolveUrl(DocumentURLProvider.GetUrl(Context.Page));
                }
            }
            var AncestorPath = await NavigationRepository.GetAncestorPathAsync(NodeAliasPath, Level, LevelIsRelative, MinimumAbsoluteLevel);
            var NavItems = await NavigationRepository.GetSecondaryNavItemsAsync(AncestorPath, Enums.PathSelectionEnum.ParentAndChildren);
            var model = new NavigationViewModel()
            {
                NavItems = NavItems.ToList(),
                CurrentPagePath = CurrentPage
            };
            
            return View(model);
        }
    }
}
