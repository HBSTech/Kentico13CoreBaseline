using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Core.Attributes
{
    /// <summary>
    /// https://andrewlock.net/post-redirect-get-using-tempdata-in-asp-net-core/
    /// </summary>
    public class ExportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Only export when ModelState is not valid
            // NOTE: we are going to always transfer in our case
            //if (!filterContext.ModelState.IsValid)
            //{
            //Export if we are redirecting
                if (filterContext.Result is RedirectResult
                    || filterContext.Result is RedirectToRouteResult
                    || filterContext.Result is RedirectToActionResult)
                {
                    var controller = filterContext.Controller as Controller;
                    if (controller != null && filterContext.ModelState != null)
                    {
                        var modelState = ModelStateHelpers.SerialiseModelState(filterContext.ModelState);
                        controller.TempData[Key] = modelState;
                    }
                }
            //}

            base.OnActionExecuted(filterContext);
        }
    }
}
