namespace Account.Features.Account.Registration
{
    [ViewComponent]
    public class RegistrationViewComponent : ViewComponent
    {
        private readonly IModelStateService _modelStateService;

        public RegistrationViewComponent(IModelStateService modelStateService )
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

            var model = _modelStateService.GetViewModel<RegistrationViewModel>(TempData).GetValueOrDefault(new RegistrationViewModel());


            return View("/Features/Account/Registration/Registration.cshtml", model);
        }
    }
}
