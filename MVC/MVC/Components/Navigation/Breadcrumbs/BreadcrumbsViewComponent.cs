using Generic.Models;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Components.Navigation.Breadcrumbs
{
    [ViewComponent(Name = "Breadcrumbs")]
    public class BreadcrumbsViewComponent : ViewComponent
    {
        private readonly IPageContextRepository _pageContextRepository;
        private readonly IBreadcrumbRepository _breadcrumbRepository;

        public BreadcrumbsViewComponent(IPageContextRepository pageContextRepository,
            IBreadcrumbRepository breadcrumbRepository)
        {
            _pageContextRepository = pageContextRepository;
            _breadcrumbRepository = breadcrumbRepository;
        }


        public async Task<IViewComponentResult> InvokeAsync(bool includeDefaultBreadcrumb = true, int nodeid = -1)
        {
            // Use current page if not provided
            if(nodeid <= 0)
            {
                var curPage = await _pageContextRepository.GetCurrentPageAsync();
                if (curPage != null)
                {
                    nodeid = curPage.NodeID;
                }
            }

            if(nodeid <= 0)
            {
                return Content(string.Empty);
            }
            var model = new BreadcrumbsViewModel()
            {
                Breadcrumbs = await _breadcrumbRepository.GetBreadcrumbsAsync(nodeid, includeDefaultBreadcrumb)
            };
            return View("Breadcrumbs", model);
        }
    }

    public record BreadcrumbsViewModel
    {
        public IEnumerable<Breadcrumb> Breadcrumbs { get; set; }
    }
}
