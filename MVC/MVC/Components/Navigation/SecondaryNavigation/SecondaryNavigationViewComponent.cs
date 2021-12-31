using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MVCCaching.Base.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Components.Navigation.SecondaryNavigation
{
    [ViewComponent(Name = "SecondaryNavigation")]
    public class SecondaryNavigationViewComponent : ViewComponent
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly ICacheDependenciesScope _cacheDependenciesScope;
        private readonly IPageContextRepository _pageContextRepository;

        public SecondaryNavigationViewComponent(INavigationRepository navigationRepository,
            ICacheDependenciesScope cacheDependenciesScope,
            IPageContextRepository pageContextRepository)
        {
            _navigationRepository = navigationRepository;
            _cacheDependenciesScope = cacheDependenciesScope;
            _pageContextRepository = pageContextRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(SecondaryNavigationProperties navigationProperties)
        {
            // Begin Cache Scope, this is 'ended' in the view
            _cacheDependenciesScope.Begin();

            navigationProperties ??= new SecondaryNavigationProperties();

            // if NodeAliasPath is empty, use current page
            if (string.IsNullOrWhiteSpace(navigationProperties.Path))
            {
                var page = await _pageContextRepository.GetCurrentPageAsync();
                if(page != null)
                {
                    navigationProperties.Path = page.Path;
                }
            }
            var ancestorPath = await _navigationRepository.GetAncestorPathAsync(navigationProperties.Path, navigationProperties.Level, navigationProperties.LevelIsRelative, navigationProperties.MinimumAbsoluteLevel);
            var navItems = await _navigationRepository.GetSecondaryNavItemsAsync(ancestorPath, Enums.PathSelectionEnum.ParentAndChildren);
            var model = new NavigationViewModel()
            {
                NavItems = navItems.ToList(),
                NavWrapperClass = navigationProperties.CssClass,
                StartingPath = ancestorPath
            };

            return View("SecondaryNavigation", model);
        }
    }

    public record SecondaryNavigationProperties()
    {
        public string Path { get; set; } = string.Empty;
        public int Level { get; set; } = 0;
        public bool LevelIsRelative { get; set; } = true;
        public int MinimumAbsoluteLevel { get; set; } = 2;
        public string CssClass { get; set; } = string.Empty;
    }
}
