using Generic.Models;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.BasicPage
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
        public IViewComponentResult Invoke(PageIdentity page)
        {
            // Any retrieval here
            var model = new BasicPageViewModel()
            {
                Page = page
            };
            return View("BasicPage", model);
        }
    }

    public record BasicPageViewModel
    {
        public PageIdentity Page;
    }
}
