using Amazon.S3.Model;

namespace Navigation.Components.Navigation.MainNavigation
{
    [ViewComponent(Name = "MainNavigation")]
    public class MainNavigationViewComponent : ViewComponent
    {
        private readonly INavigationRepository _navigationRepository;

        public MainNavigationViewComponent(INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(string NavigationParentPath, string CssClass = "MainNav")
        {
            NavigationParentPath = !string.IsNullOrWhiteSpace(NavigationParentPath) ? NavigationParentPath : "/MasterPage/Navigation";
            var NavItems = await _navigationRepository.GetNavItemsAsync(NavigationParentPath);
            var model = new NavigationViewModel(
                navItems: NavItems.ToList(),
                navWrapperClass: CssClass
            );

            return View("/Components/MainNavigation/MainNavigation.cshtml", model);
        }
        public record NavigationViewModel
        {
            public NavigationViewModel(IEnumerable<NavigationItem> navItems, string navWrapperClass)
            {
                NavItems = navItems;
                NavWrapperClass = navWrapperClass;
            }

            public IEnumerable<NavigationItem> NavItems { get; set; } = Array.Empty<NavigationItem>();
            public string NavWrapperClass { get; set; }
        }
    }
}
