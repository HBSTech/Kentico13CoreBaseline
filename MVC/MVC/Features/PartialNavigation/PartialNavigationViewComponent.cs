using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.PartialNavigation
{
    [ViewComponent]
    public class PartialNavigationViewComponent : ViewComponent
    {
        public PartialNavigationViewComponent()
        {
        }


        public IViewComponentResult Invoke()
        {
            var model = new PartialNavigationViewModel()
            {

            };
            return View("PartialNavigation", model);
        }
    }

    public record PartialNavigationViewModel
    {

    }
}
