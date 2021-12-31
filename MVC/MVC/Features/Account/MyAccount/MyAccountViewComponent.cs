using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Account.MyAccount
{
    [ViewComponent]
    public class MyAccountViewComponent : ViewComponent
    {

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke(MyAccountViewModel model)
        {
            model ??= new MyAccountViewModel();

            return View("~/Features/Account/MyAccount/MyAccount.cshtml", model);
        }
    }
}
