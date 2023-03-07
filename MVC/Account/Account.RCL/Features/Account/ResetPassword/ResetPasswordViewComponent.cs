namespace Account.Features.Account.ResetPassword
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
            _modelStateService.MergeModelState(ModelState, TempData);
            var model = _modelStateService.GetViewModel<ResetPasswordViewModel>(TempData).GetValueOrDefault(new ResetPasswordViewModel());

            return View("/Features/Account/ResetPassword/ResetPassword.cshtml", model);
        }
    }
}
