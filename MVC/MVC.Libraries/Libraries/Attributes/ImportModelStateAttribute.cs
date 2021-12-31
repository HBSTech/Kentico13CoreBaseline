using Generic.Libraries.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Generic.Libraries.Attributes
{
    /// <summary>
    /// https://andrewlock.net/post-redirect-get-using-tempdata-in-asp-net-core/
    /// </summary>
    public class ImportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            var serialisedModelState = controller?.TempData[Key] as string;

            if (serialisedModelState != null)
            {
                //Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    var modelState = ModelStateHelpers.DeserialiseModelState(serialisedModelState);
                    filterContext.ModelState.Merge(modelState);
                }
                else
                {
                    //Otherwise remove it.
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
