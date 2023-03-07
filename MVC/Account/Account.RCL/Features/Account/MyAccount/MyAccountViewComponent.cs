using Microsoft.AspNetCore.Mvc;

namespace Account.Features.Account.MyAccount
{
    [ViewComponent]
    public class MyAccountViewComponent : ViewComponent
    {

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            // Nothing for IModelStateService to be required

            var model = new MyAccountViewModel();

            return View("/Features/Account/MyAccount/MyAccount.cshtml", model);
        }
    }
}
