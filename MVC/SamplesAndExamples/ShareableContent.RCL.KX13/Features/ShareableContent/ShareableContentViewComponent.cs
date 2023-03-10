using Microsoft.AspNetCore.Mvc;

namespace ShareableContent.Features.ShareableContent
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
            return View("/Features/ShareableContent/ShareableContent.cshtml", model);
        }
    }

    public record ShareableContentViewModel
    {
    }
}
