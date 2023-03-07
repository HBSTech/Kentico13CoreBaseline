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
                includeCurrentPageSelector: navigationProperties.IncludeSecondaryNavSelector
            );

            return View("SecondaryNavigation", model);
        }
    }

    public record SecondaryNavigationProperties()
    {
        /// <summary>
        /// The Path that the navigation properties build off of.  If empty or not provided, will use the current page's path.
        /// </summary>
        public Maybe<string> Path { get; set; }

        /// <summary>
        /// The level the navigation should start at.  0 = At the root (if LevelIsRelative is false), or 0 = Current page's level (if LevelIsRelative is true)
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// If the given Level is relative to the current page's level.  If true, then the Level dictates what parent is the start point.  A level of 2 will go up 2 levels. 
        /// </summary>
        public bool LevelIsRelative { get; set; } = true;

        /// <summary>
        /// How many levels down from the start of the navigation it should show entries.
        /// </summary>
        public int MinimumAbsoluteLevel { get; set; } = 2;

        /// <summary>
        /// The CSS class that will wrap this navigation.  useful both for styling and for the Navigation Page Selector
        /// </summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>
        /// If true, will include the client-side javascript that sets the active
        /// </summary>
        public bool IncludeSecondaryNavSelector { get; set; } = true;
    }
}
