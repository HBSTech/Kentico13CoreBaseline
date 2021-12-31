using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Account.ResetPassword
{
    [ViewComponent]
    public class ResetPasswordViewComponent : ViewComponent
    {
        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke(ResetPasswordViewModel model)
        {
            model ??= new ResetPasswordViewModel();

            return View("~/Features/Account/ResetPassword/ResetPassword.cshtml", model);
        }
    }
}
