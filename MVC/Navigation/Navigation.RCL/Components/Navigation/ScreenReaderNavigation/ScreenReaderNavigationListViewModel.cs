namespace Navigation.Components.Navigation.ScreenReaderNavigation
{
    public class ScreenReaderNavigationListViewModel
    {
        public ScreenReaderNavigationListViewModel(NavigationItem navItem, int level)
        {
            NavItem = navItem;
            Level = level;
        }
        public NavigationItem NavItem { get; set; }
        public int Level { get; set; }
    }
}
