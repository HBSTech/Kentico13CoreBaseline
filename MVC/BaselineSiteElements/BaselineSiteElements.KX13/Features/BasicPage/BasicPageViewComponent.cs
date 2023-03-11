namespace BaselineSiteElements.Features.BasicPage
{
    [ViewComponent]
    public class BasicPageViewComponent : ViewComponent
    {
        public BasicPageViewComponent()
        {
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke(PageIdentity xPage)
        {
            // Any retrieval here
            var model = new BasicPageViewModel(
                page: xPage
            );
            return View("/Features/BasicPage/BasicPage.cshtml", model);
        }
    }

    public record BasicPageViewModel
    {
        public PageIdentity Page;

        public BasicPageViewModel(PageIdentity page)
        {
            Page = page;
        }
    }
}
