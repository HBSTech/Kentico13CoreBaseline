using Microsoft.AspNetCore.Mvc;

namespace Navigation.Features.PartialNavigation
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
            return View("/Features/Navigation/PartialNavigation/PartialNavigation.cshtml", model);
        }
    }

    public record PartialNavigationViewModel
    {

    }
}
