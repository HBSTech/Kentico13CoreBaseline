using Microsoft.AspNetCore.Mvc;
using XperienceCommunity.Authorization;

namespace Account.Features.Account.MyAccount
{
    public class MyAccountController : Controller
    {
        // If Adjusted, also adjust LogInController.cs as well
        public const string _routeUrl = "Account/MyAccount";

        public MyAccountController()
        {
        }

        /// <summary>
        /// Can enable and create a My Account View.
        /// </summary>
        [HttpGet]
        [Route(MyAccountControllerPath._routeUrl)]
        [ControllerActionAuthorization(userAuthenticationRequired: true)]
        public ActionResult MyAccount()
        {
            return View("/Features/Account/MyAccount/MyAccountManual.cshtml");
        }

    }
}
