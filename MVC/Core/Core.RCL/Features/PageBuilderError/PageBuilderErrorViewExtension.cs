using Core.Features.PageBuilderError;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ViewComponentExtensions
    {
        public static ViewViewComponentResult PageBuilderMessage(this ViewComponent component, string message, bool inline = true, bool isError = true)
        {
            var model = new PageBuilderErrorViewModel(message, inline, isError);
            return component.View("/Features/PageBuilderError/Message.cshtml", model);
        }
    }
}
