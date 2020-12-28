using CMS.DocumentEngine.Types.Generic;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Generic.Components.Partials
{
    [ViewComponent(Name = "PartialFooter")]
    public class PartialFooterViewComponent : ViewComponent
    {
        public PartialFooterViewComponent(IPageDataContextRetriever pageDataContextRetriever)
        {
            PageDataContextRetriever = pageDataContextRetriever;
        }

        public IPageDataContextRetriever PageDataContextRetriever { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if(PageDataContextRetriever.TryRetrieve(out IPageDataContext<Footer> data))
            {
                return View("~/Views/Shared/Components/PartialFooter/Default.cshtml", data.Page);
            } else
            {
                return Content("<!-- Page not found, could not render -->");
            }
        }
    }
}
