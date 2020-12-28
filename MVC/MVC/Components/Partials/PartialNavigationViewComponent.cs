using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Generic.Components.Partials
{
    [ViewComponent(Name = "PartialNavigation")]
    public class PartialNavigationViewComponent : ViewComponent
    {
        public PartialNavigationViewComponent(IPageDataContextRetriever pageDataContextRetriever)
        {
            PageDataContextRetriever = pageDataContextRetriever;
        }

        public IPageDataContextRetriever PageDataContextRetriever { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (PageDataContextRetriever.TryRetrieve(out IPageDataContext<CMS.DocumentEngine.Types.Generic.Navigation> data))
            {
                return View("~/Views/Shared/Components/PartialNavigation/Default.cshtml", data.Page);
            }
            else
            {
                return Content("<!-- Page not found, could not render -->");
            }
        }
    }
}
