using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.ShareableContent
{
    [ViewComponent]
    public class ShareableContentViewComponent : ViewComponent
    {

        public ShareableContentViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            var model = new ShareableContentViewModel() {  };
            return View("ShareableContent", model);
        }
    }

    public record ShareableContentViewModel
    {
    }
}
