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
        public IViewComponentResult Invoke(string path)
        {
            // Any retrieval here
            var model = new BasicPageViewModel()
            {
                Path = path
            };
            return View("BasicPage", model);
        }
    }

    public record BasicPageViewModel
    {
        public string Path;
    }
}
