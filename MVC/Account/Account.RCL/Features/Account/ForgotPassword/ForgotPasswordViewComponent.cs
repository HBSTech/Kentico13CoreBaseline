using Microsoft.AspNetCore.Mvc;

namespace Account.Features.Account.ForgotPassword
{
    [ViewComponent]
    public class ForgotPasswordViewComponent : ViewComponent
    {
        private readonly IModelStateService _modelStateService;

        public ForgotPasswordViewComponent(IModelStateService modelStateService)
        {
            _modelStateService = modelStateService;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            // Hydrate Model State
            _modelStateService.MergeModelState(ModelState, TempData);

            // Get View Model State
            var model = _modelStateService.GetViewModel<ForgotPasswordViewModel>(TempData).GetValueOrDefault(new ForgotPasswordViewModel());

            return View("/Features/Account/ForgotPassword/ForgotPassword.cshtml", model);
        }
    }
}
