using CMS.DocumentEngine.Types.Generic;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Generic.Components.Partials
{
    [ViewComponent(Name = "PartialHeader")]
    public class PartialHeaderViewComponent : ViewComponent
    {
        public PartialHeaderViewComponent(IPageDataContextRetriever pageDataContextRetriever)
        {
            PageDataContextRetriever = pageDataContextRetriever;
        }

        public IPageDataContextRetriever PageDataContextRetriever { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (PageDataContextRetriever.TryRetrieve(out IPageDataContext<Header> data))
            {
                return View("~/Views/Shared/Components/PartialHeader/Default.cshtml", data.Page);
            }
            else
            {
                return Content("<!-- Page not found, could not render -->");
            }
        }
    }
}
