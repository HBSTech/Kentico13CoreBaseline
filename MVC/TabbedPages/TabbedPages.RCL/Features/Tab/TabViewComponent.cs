namespace TabbedPages.Features.Tab
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
            return View("/Features/Tab/Tab.cshtml", model);
        }
    }

    public record TabViewModel
    {

    }
}
