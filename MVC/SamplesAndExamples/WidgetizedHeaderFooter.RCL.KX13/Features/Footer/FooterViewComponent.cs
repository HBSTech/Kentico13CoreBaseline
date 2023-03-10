namespace WidgetizedHeaderFooter.Features.Footer
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
            return View("/Features/Footer/Footer.cshtml", model);
        }
    }

    public record FooterViewModel
    {

    }
}
