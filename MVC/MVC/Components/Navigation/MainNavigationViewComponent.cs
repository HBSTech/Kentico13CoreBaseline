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
    [ViewComponent(Name = "MainNavigation")]
    public class MainNavigationViewComponent : ViewComponent
    {
        public INavigationRepository NavigationRepository { get; }
        public IPageDataContextRetriever DataRetriever { get; }
        public IUrlHelper UrlHelper { get; }

        public MainNavigationViewComponent(INavigationRepository navigationRepository, IPageDataContextRetriever dataRetriever, IUrlHelper urlHelper)
        {
            NavigationRepository = navigationRepository;
            DataRetriever = dataRetriever;
            UrlHelper = urlHelper;
        }
        public async Task<IViewComponentResult> InvokeAsync(string NavigationParentPath, string CurrentPage = null)
        {
            // Use ViewBag to set current Path
            if (!string.IsNullOrWhiteSpace(CurrentPage))
            {
                if (CurrentPage == "/Home")
                {
                    // Special case for Home since the Link is just "/"
                    CurrentPage = "/";
                }
            } else
            {
                if (DataRetriever.TryRetrieve(out IPageDataContext<TreeNode> Context))
                {
                    CurrentPage = UrlHelper.Content(DocumentURLProvider.GetUrl(Context.Page));
                }
            }

            NavigationParentPath = !string.IsNullOrWhiteSpace(NavigationParentPath) ? NavigationParentPath : "/MasterPage/Navigation";
            var NavItems = await NavigationRepository.GetNavItemsAsync(NavigationParentPath);
            var model = new NavigationViewModel()
            {
                NavItems = NavItems.ToList(),
                CurrentPagePath = CurrentPage
            };


            return View(model);
        } 
    }
}
