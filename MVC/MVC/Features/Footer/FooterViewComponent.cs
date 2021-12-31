using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Footer
{
    [ViewComponent]
    public class FooterViewComponent : ViewComponent
    {
        public FooterViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            var model = new FooterViewModel()
            {

            };
            return View("Footer", model);
        }
    }

    public record FooterViewModel
    {

    }
}
