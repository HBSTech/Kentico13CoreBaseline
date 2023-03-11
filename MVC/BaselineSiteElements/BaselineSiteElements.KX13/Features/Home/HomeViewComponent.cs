
namespace BaselineSiteElements.Features.Home
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
            return View("/Features/Home/Home.cshtml", model);
        }
    }

    public record HomeViewModel
    {

    }
}
