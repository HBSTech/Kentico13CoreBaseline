using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    public class HttpErrorsController : Controller
    {
        public ActionResult Error(int code)
        {
            switch(code)
            {
                case 404:
                    return Error404();
                case 500:
                    return Error500();
                case 403:
                    return AccessDenied();
            }
            return View(code);
        }

        public ActionResult Error404()
        {
            Response.StatusCode = 404;
            return View("Error404");
        }

        public ActionResult Error500()
        {
            Response.StatusCode = 500;
            return View("Error500");
        }

        public ActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View("AccessDenied");
        }


        // Testing
        public ActionResult Test500()
        {
            throw new System.Exception("This is a test of a 500 error");
        }

    }
}