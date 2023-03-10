namespace WidgetizedHeaderFooter.Features.Header
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
            return View("/Features/Header/Header.cshtml", model);

        }
    }

    public record HeaderViewModel
    {

    }
}
