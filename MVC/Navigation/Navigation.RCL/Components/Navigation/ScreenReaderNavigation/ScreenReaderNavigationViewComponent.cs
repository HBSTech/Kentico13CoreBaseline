namespace Navigation.Components.Navigation.ScreenReaderNavigation
{
    public class ScreenReaderNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<NavigationItem> navigationItems, string navigationId)
        {
            return View("/Components/Navigation/ScreenReaderNavigation/ScreenReaderNavigation.cshtml", new ScreenReaderNavigationViewModel(navigationItems, navigationId));
        }
    }

    public record ScreenReaderNavigationViewModel
    {
        public ScreenReaderNavigationViewModel(IEnumerable<NavigationItem> navigationItems, string id)
        {
            NavigationItems = navigationItems;
            Id = id;
        }

        public IEnumerable<NavigationItem> NavigationItems { get; set; }
        public string Id { get; set; }
    }
}
