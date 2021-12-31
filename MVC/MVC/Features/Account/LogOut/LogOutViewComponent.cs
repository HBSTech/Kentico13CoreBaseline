using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Account.LogOut
{
    [ViewComponent]
    public class LogOutViewComponent : ViewComponent
    {
        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke(LogOutViewModel model = null)
        {
            // Any retrieval here
            model ??= new LogOutViewModel()
            {
                
            };
            return View("~/Features/Account/LogOut/LogOut.cshtml", model);
        }
    }
}
