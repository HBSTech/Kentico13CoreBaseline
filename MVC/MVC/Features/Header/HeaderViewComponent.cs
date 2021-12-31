using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Header
{
    [ViewComponent]
    public class HeaderViewComponent : ViewComponent
    {
        public HeaderViewComponent()
        {

        }

        public IViewComponentResult Invoke()
        {
            var model = new HeaderViewModel()
            {

            };
            return View("Header", model);

        }
    }

    public record HeaderViewModel
    {

    }
}
