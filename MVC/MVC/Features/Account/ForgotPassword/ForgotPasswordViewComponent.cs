using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Account.ForgotPassword
{
    [ViewComponent]
    public class ForgotPasswordViewComponent : ViewComponent
    {

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke(ForgotPasswordViewModel model)
        {
            model ??= new ForgotPasswordViewModel();

            return View("~/Features/Account/ForgotPassword/ForgotPassword.cshtml", model);
        }
    }
}
