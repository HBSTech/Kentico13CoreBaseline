using Microsoft.AspNetCore.Mvc;

namespace Account.Features.Account.Confirmation
{
    public class ConfirmationController : Controller
    {
        public const string _routeUrl = "Account/Confirmation";

        /// <summary>
        /// Fallback if not using Page Templates
        /// </summary>
        [HttpGet]
        [Route(_routeUrl)]
        public ActionResult Confirmation()
        {
            return View("/Features/Account/Confirmation/ConfirmationManual.cshtml");
        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
