using Microsoft.AspNetCore.Mvc;

namespace Generic.Components.Navigation.MegaMenu
{
    [ViewComponent(Name = "MegaMenu")]
    public class MegaMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("MegaMenu");
        }
    }
}
