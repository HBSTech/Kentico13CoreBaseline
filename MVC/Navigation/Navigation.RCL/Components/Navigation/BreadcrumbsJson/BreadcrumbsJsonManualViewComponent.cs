using System.Text.Json;

namespace Navigation.Components.Navigation.BreadcrumbsJson
{
    [ViewComponent]
    public class BreadcrumbsJsonManualViewComponent : ViewComponent
    {
        private readonly IBreadcrumbRepository _breadcrumbRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BreadcrumbsJsonManualViewComponent(IBreadcrumbRepository breadcrumbRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _breadcrumbRepository = breadcrumbRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Breadcrumb> breadcrumbs, bool includeDefaultBreadcrumb = true)
        {
            if(_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext))
            {
                httpContext.Items.TryAdd("BreadcrumbJsonLDManuallyDone", true);
            }
            var breadcrumbsList = breadcrumbs.ToList();
            if(includeDefaultBreadcrumb)
            {
                breadcrumbsList.Insert(0, await _breadcrumbRepository.GetDefaultBreadcrumbAsync());
            }
            var breadcrumbsJson = await _breadcrumbRepository.BreadcrumbsToJsonLDAsync(breadcrumbsList, !includeDefaultBreadcrumb);
            // Serialize into the raw JSON data
            var model = new BreadcrumbsJsonViewModel(
                serializedBreadcrumbJsonLD: JsonSerializer.Serialize(breadcrumbsJson)
            );
            return View("/Components/Navigation/BreadcrumbsJson/BreadcrumbsJson.cshtml", model);
        }
    }
}
