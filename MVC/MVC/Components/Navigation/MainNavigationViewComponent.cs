using Generic.Repositories.Interfaces;
using Generic.ViewModels;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using MVCCaching.Base.Core.Interfaces;
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
        public ICacheDependenciesStore CacheDependenciesStore { get; }
        public ICacheDependenciesScope CacheDependenciesScope { get; }

        public MainNavigationViewComponent(INavigationRepository navigationRepository, IPageDataContextRetriever dataRetriever, IUrlHelper urlHelper, ICacheDependenciesStore cacheDependenciesStore, ICacheDependenciesScope cacheDependenciesScope)
        {
            NavigationRepository = navigationRepository;
            DataRetriever = dataRetriever;
            UrlHelper = urlHelper;
            CacheDependenciesStore = cacheDependenciesStore;
            CacheDependenciesScope = cacheDependenciesScope;
        }
        public async Task<IViewComponentResult> InvokeAsync(string NavigationParentPath, string CssClass = "MainNav")
        {
            NavigationParentPath = !string.IsNullOrWhiteSpace(NavigationParentPath) ? NavigationParentPath : "/MasterPage/Navigation";
            var NavItems = await NavigationRepository.GetNavItemsAsync(NavigationParentPath);
            var model = new NavigationViewModel()
            {
                NavItems = NavItems.ToList(),
                NavWrapperClass = CssClass
            };

            return View(model);
        }
    }
}
