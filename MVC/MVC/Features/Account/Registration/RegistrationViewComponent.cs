using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Account.Registration
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

            var model = _modelStateService.GetViewModel<RegistrationViewModel>(TempData) ?? new RegistrationViewModel()
            {

            };


            return View("~/Features/Account/Registration/Registration.cshtml", model);
        }

        public static IHtmlContent FooterContent()
        {
            return new HtmlString(@"
                <script src=""~/Scripts/jquery-1.10.2.min.js""></script>
                <script src=""~/Scripts/jquery.validate.min.js""></script>
                <script src=""~/Scripts/jquery.validate.unobtrusive.min.js""></script>");
        }
    }
}
