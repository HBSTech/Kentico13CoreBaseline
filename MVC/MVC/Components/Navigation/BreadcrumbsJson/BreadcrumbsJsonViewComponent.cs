using Generic.Models;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Generic.Components.Navigation.BreadcrumbsJson
{
    [ViewComponent(Name = "BreadcrumbsJson")]
    public class BreadcrumbsJsonViewComponent : ViewComponent
    {
        private readonly IPageContextRepository _pageContextRepository;
        private readonly IBreadcrumbRepository _breadcrumbRepository;

        public BreadcrumbsJsonViewComponent(IPageContextRepository pageContextRepository,
            IBreadcrumbRepository breadcrumbRepository)
        {
            _pageContextRepository = pageContextRepository;
            _breadcrumbRepository = breadcrumbRepository;
        }


        public async Task<IViewComponentResult> InvokeAsync(bool IncludeDefaultBreadcrumb = true, int Nodeid = -1)
        {
            // Use current page if not provided
            if(Nodeid <= 0)
            {
                var currentPage = await _pageContextRepository.GetCurrentPageAsync();
                if (currentPage != null)
                {
                    Nodeid = currentPage.NodeID;
                }
            }

            if(Nodeid <= 0)
            {
                return Content(string.Empty);
            }
            var breadcrumbs = await _breadcrumbRepository.GetBreadcrumbsAsync(Nodeid, IncludeDefaultBreadcrumb);
            var breadcrumbsJson = await _breadcrumbRepository.BreadcrumbsToJsonLDAsync(breadcrumbs, !IncludeDefaultBreadcrumb);
            // Serialize into the raw JSON data
            var model = new BreadcrumbsJsonViewModel()
            {
                SerializedBreadcrumbJsonLD = JsonConvert.SerializeObject(breadcrumbsJson)
            };
            return View("BreadcrumbsJson", model);
        }
    }

    public record BreadcrumbsJsonViewModel
    {
        public string SerializedBreadcrumbJsonLD { get; set; }
    }
}
