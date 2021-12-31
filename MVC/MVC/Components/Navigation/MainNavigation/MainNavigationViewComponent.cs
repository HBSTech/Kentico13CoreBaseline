using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Components.Navigation.MainNavigation
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
            var model = new NavigationViewModel()
            {
                NavItems = NavItems.ToList(),
                NavWrapperClass = CssClass
            };

            return View("MainNavigation", model);
        }
    }
}
