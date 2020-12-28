using CMS.DocumentEngine;
using Generic.Repositories.Interfaces;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Generic.Components
{
    [ViewComponent(Name = "Breadcrumbs")]
    public class BreadcrumbsViewComponent : ViewComponent
    {
        public BreadcrumbsViewComponent(IPageDataContextRetriever pageDataContextRetriever,
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
            var model = await BreadcrumbRepository.GetBreadcrumbsAsync(Nodeid, IncludeDefaultBreadcrumb);
            return View(model);
        }
    }
}
