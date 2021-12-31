using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Tab
{
    [ViewComponent]
    public class TabViewComponent : ViewComponent
    {
        public TabViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            var model = new TabViewModel()
            {

            };
            return View("Tab", model);
        }
    }

    public record TabViewModel
    {

    }
}
