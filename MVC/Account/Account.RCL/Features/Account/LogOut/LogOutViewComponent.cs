using Microsoft.AspNetCore.Mvc;

namespace Account.Features.Account.LogOut
{
    [ViewComponent]
    public class LogOutViewComponent : ViewComponent
    {
        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            // Nothing in View Model to need IModelStateService to restore

            // Any retrieval here
            var model = new LogOutViewModel()
            {
                
            };
            return View("/Features/Account/LogOut/LogOut.cshtml", model);
        }
    }
}
