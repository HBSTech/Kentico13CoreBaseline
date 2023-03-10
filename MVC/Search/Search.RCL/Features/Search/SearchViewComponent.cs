using Core.Repositories;
using Microsoft.Extensions.Primitives;
using Search.Repositories;
namespace Search.Features.Search
{
    
    [ViewComponent]
    public class SearchViewComponent : ViewComponent
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISiteRepository _siteRepository;

        public SearchViewComponent(ISearchRepository searchRepository,
            IHttpContextAccessor httpContextAccessor,
            ISiteRepository siteRepository)
        {
            _searchRepository = searchRepository;
            _httpContextAccessor = httpContextAccessor;
            _siteRepository = siteRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get values from Query String
            Maybe<string> searchValue = Maybe.None;
            int page = 1;
            int pageSize = 100;
            if (_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext))
            {
                if (httpContext.Request.Query.TryGetValue("searchValue", out StringValues querySearchValue) && querySearchValue.Any())
                {
                    searchValue = querySearchValue.First();
                }
                if (httpContext.Request.Query.TryGetValue("page", out StringValues queryPage) && queryPage.Any())
                {
                    _ = int.TryParse(queryPage.First(), out page);
                }
                if (httpContext.Request.Query.TryGetValue("pageSize", out StringValues queryPageSize) && queryPageSize.Any())
                {
                    _ = int.TryParse(queryPageSize.First(), out pageSize);
                }
            }

            var model = new SearchViewModel(
                searchValue: searchValue.GetValueOrDefault(string.Empty),
                currentPage: page,
                pageSize: pageSize
            );

            if (searchValue.TryGetValue(out var searchVal))
            {
                var indexes = new string[] { "SearchIndexName" };

                // Perform search
                model.SearchResults = await _searchRepository.Search(searchVal, indexes, page, pageSize);
            }
            return View("/Features/Search/Search.cshtml", model);
        }
    }

    public record SearchViewModel
    {
        public SearchViewModel(string searchValue, int currentPage, int pageSize)
        {
            SearchValue = searchValue;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        public string SearchValue { get; set; }
        public Maybe<SearchResponse> SearchResults { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}
