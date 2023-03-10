using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Generic.Features.Home
{
    [ViewComponent]
    public class HomeViewComponent : ViewComponent
    {

        public HomeViewComponent()
        {
            
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            // Any retrieval here
            var model = new HomeViewModel()
            {
                // Properties here
            };
            return View("Home", model);
        }
    }

    public record HomeViewModel
    {

    }
}
