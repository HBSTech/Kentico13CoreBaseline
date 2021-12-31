using Microsoft.AspNetCore.Mvc;
using XperienceCommunity.Authorization;

namespace Generic.Features.Account.MyAccount
{
    public class MyAccountController : Controller
    {
        public const string _routeUrl = "Account/MyAccount";

        public MyAccountController()
        {
        }

        /// <summary>
        /// Can enable and create a My Account View.
        /// </summary>
        [HttpGet]
        [Route(_routeUrl)]
        [ControllerActionAuthorization(userAuthenticationRequired: true)]
        public ActionResult MyAccount()
        {
            return View("~/Features/Account/MyAccount/MyAccountManual.cshtml");
        }

        public static string GetUrl()
        {
            return "/" + _routeUrl;
        }
    }
}
