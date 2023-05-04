using MVCCaching;

namespace Navigation.Components.Navigation.SecondaryNavigation
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
            if (navigationProperties.Path.HasNoValue)
            {
                var page = await _pageContextRepository.GetCurrentPageAsync();
                if (page.TryGetValue(out var pageItem))
                {
                    navigationProperties.Path = pageItem.Path;
                }
            }

            // If include secondary navigation, need a css class
            if (navigationProperties.IncludeSecondaryNavSelector && !string.IsNullOrWhiteSpace(navigationProperties.CssClass))
            {
                navigationProperties.CssClass = "secondary-navigation";
            }

            var ancestorPath = await _navigationRepository.GetAncestorPathAsync(navigationProperties.Path.GetValueOrDefault("/"), navigationProperties.Level, navigationProperties.LevelIsRelative, navigationProperties.MinimumAbsoluteLevel);
            var navItems = await _navigationRepository.GetSecondaryNavItemsAsync(ancestorPath, Enums.PathSelectionEnum.ParentAndChildren);
            var model = new NavigationViewModel(

                navItems: navItems.ToList(),
                navWrapperClass: navigationProperties.CssClass,
                startingPath: ancestorPath,
                currentPagePath: navigationProperties.Path.GetValueOrDefault("/"),
                includeCurrentPageSelector: navigationProperties.IncludeSecondaryNavSelector,
                includeScreenReaderNavigation: navigationProperties.IncludeScreenReaderNav
            );

            return View("SecondaryNavigation", model);
        }
    }
}
