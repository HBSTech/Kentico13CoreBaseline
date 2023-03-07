using System.Text.Json;

namespace Navigation.Components.Navigation.BreadcrumbsJson
{
    [ViewComponent(Name = "BreadcrumbsJson")]
    public class BreadcrumbsJsonViewComponent : ViewComponent
    {
        private readonly IPageContextRepository _pageContextRepository;
        private readonly IBreadcrumbRepository _breadcrumbRepository;
        private readonly IHttpContextAccessor _httpContext;

        public BreadcrumbsJsonViewComponent(IPageContextRepository pageContextRepository,
            IBreadcrumbRepository breadcrumbRepository,
            IHttpContextAccessor httpContext)
        {
            _pageContextRepository = pageContextRepository;
            _breadcrumbRepository = breadcrumbRepository;
            _httpContext = httpContext;
        }


        public async Task<IViewComponentResult> InvokeAsync(bool IncludeDefaultBreadcrumb = true, int Nodeid = -1)
        {
            if(_httpContext.HttpContext.AsMaybe().TryGetValue(out var httpContext)
                && httpContext.Items.ContainsKey("BreadcrumbJsonLDManuallyDone"))
            {
                return Content(String.Empty);
            }
            // Use current page if not provided
            if(Nodeid <= 0)
            {
                var currentPage = await _pageContextRepository.GetCurrentPageAsync();
                if (currentPage.TryGetValue(out var curPage))
                {
                    Nodeid = curPage.NodeID;
                }
            }

            if(Nodeid <= 0)
            {
                return Content(string.Empty);
            }
            var breadcrumbs = await _breadcrumbRepository.GetBreadcrumbsAsync(Nodeid, IncludeDefaultBreadcrumb);
            var breadcrumbsJson = await _breadcrumbRepository.BreadcrumbsToJsonLDAsync(breadcrumbs, !IncludeDefaultBreadcrumb);
            // Serialize into the raw JSON data
            var model = new BreadcrumbsJsonViewModel(
                serializedBreadcrumbJsonLD: JsonSerializer.Serialize(breadcrumbsJson)
            );
            return View("/Components/Navigation/BreadcrumbsJson/BreadcrumbsJson.cshtml", model);
        }
    }
}
