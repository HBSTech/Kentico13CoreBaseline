using Microsoft.AspNetCore.Mvc;

namespace Generic.Features.Account.Registration
{
    [ViewComponent]
    public class RegistrationViewComponent : ViewComponent
    {
        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke(RegistrationViewModel model = null)
        {
            // Any retrieval here
            model ??= new RegistrationViewModel()
            {
                
            };
            return View("~/Features/Account/Registration/Registration.cshtml", model);
        }
    }
}
