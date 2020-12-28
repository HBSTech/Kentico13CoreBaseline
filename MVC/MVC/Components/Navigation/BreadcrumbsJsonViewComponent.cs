using CMS.DocumentEngine;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Generic.Components
{
    [ViewComponent(Name = "BreadcrumbsJson")]
    public class BreadcrumbsJsonViewComponent : ViewComponent
    {
        public BreadcrumbsJsonViewComponent(IPageDataContextRetriever pageDataContextRetriever,
            IBreadcrumbRepository breadcrumbRepository)
        {
            PageDataContextRetriever = pageDataContextRetriever;
            BreadcrumbRepository = breadcrumbRepository;
        }

        public IPageDataContextRetriever PageDataContextRetriever { get; }
        public IBreadcrumbRepository BreadcrumbRepository { get; }

        public async Task<IViewComponentResult> InvokeAsync(bool IncludeDefaultBreadcrumb = true, int Nodeid = -1)
        {
            // Use current page if not provided
            if(Nodeid <= 0)
            {
                if(PageDataContextRetriever.TryRetrieve(out IPageDataContext<TreeNode> Context)){
                    Nodeid = Context.Page.NodeID;
                }
            }

            if(Nodeid <= 0)
            {
                return Content(string.Empty);
            }
            var breadcrumbs = await BreadcrumbRepository.GetBreadcrumbsAsync(Nodeid, IncludeDefaultBreadcrumb);
            var model = await BreadcrumbRepository.BreadcrumbsToJsonLDAsync(breadcrumbs, !IncludeDefaultBreadcrumb);
            // Serialize into the raw JSON data
            model.JsonData = JsonConvert.SerializeObject(model);
            return View(model);
        }
    }
}
