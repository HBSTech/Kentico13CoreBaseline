using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Account.ResetPassword
{
    [ViewComponent]
    public class ResetPasswordViewComponent : ViewComponent
    {
        private readonly IModelStateService _modelStateService;

        public ResetPasswordViewComponent(IModelStateService modelStateService)
        {
            _modelStateService = modelStateService;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            var model = _modelStateService.GetViewModel<ResetPasswordViewModel>(TempData) ?? new ResetPasswordViewModel();

            return View("~/Features/Account/ResetPassword/ResetPassword.cshtml", model);
        }
    }
}
