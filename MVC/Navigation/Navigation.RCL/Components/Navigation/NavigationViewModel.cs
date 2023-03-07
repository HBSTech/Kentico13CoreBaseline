namespace Navigation.Components.Navigation
{
    public class NavigationViewModel
    {
        public NavigationViewModel(List<NavigationItem> navItems, string navWrapperClass, string startingPath, string currentPagePath, bool includeCurrentPageSelector)
        {
            NavItems = navItems;
            NavWrapperClass = navWrapperClass;
            StartingPath = startingPath;
            CurrentPagePath = currentPagePath;
            IncludeCurrentPageSelector = includeCurrentPageSelector;
        }

        public List<NavigationItem> NavItems { get; set; }
        public string NavWrapperClass { get; set; }
        public string StartingPath { get; set; }
        public string CurrentPagePath { get; set; }
        public bool IncludeCurrentPageSelector { get; set; }
    }
}