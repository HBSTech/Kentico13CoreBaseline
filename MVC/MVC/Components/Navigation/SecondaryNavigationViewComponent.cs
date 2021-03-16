using Generic.Repositories.Interfaces;
using Generic.ViewModels;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using MVCCaching.Base.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Components
{
    [ViewComponent(Name = "SecondaryNavigation")]
    public class SecondaryNavigationViewComponent : ViewComponent
    {
        public SecondaryNavigationViewComponent(INavigationRepository navigationRepository, IPageDataContextRetriever pageDataContextRetriever, IUrlResolver urlResolver, ICacheDependenciesStore cacheDependenciesStore, ICacheDependenciesScope cacheDependenciesScope)
        {
            NavigationRepository = navigationRepository;
            PageDataContextRetriever = pageDataContextRetriever;
            UrlResolver = urlResolver;
            CacheDependenciesStore = cacheDependenciesStore;
            CacheDependenciesScope = cacheDependenciesScope;
        }

        public INavigationRepository NavigationRepository { get; }
        public IPageDataContextRetriever PageDataContextRetriever { get; }
        public IUrlResolver UrlResolver { get; }
        public ICacheDependenciesStore CacheDependenciesStore { get; }
        public ICacheDependenciesScope CacheDependenciesScope { get; }

        public async Task<IViewComponentResult> InvokeAsync(string NodeAliasPath, int Level, bool LevelIsRelative = true, int MinimumAbsoluteLevel = 2, string CssClass = null)
        {
            // Begin Cache Scope, this is 'ended' in the view
            CacheDependenciesScope.Begin();

            var AncestorPath = await NavigationRepository.GetAncestorPathAsync(NodeAliasPath, Level, LevelIsRelative, MinimumAbsoluteLevel);
            var NavItems = await NavigationRepository.GetSecondaryNavItemsAsync(AncestorPath, Enums.PathSelectionEnum.ParentAndChildren);
            var model = new NavigationViewModel()
            {
                NavItems = NavItems.ToList(),
                NavWrapperClass = CssClass,
                StartingPath = AncestorPath
            };

            return View(model);
        }
    }
}
