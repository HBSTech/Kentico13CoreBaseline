using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Generic.Components.Navigation
{
    [ViewComponent(Name = "MegaMenu")]
    public class MegaMenuViewComponent : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
