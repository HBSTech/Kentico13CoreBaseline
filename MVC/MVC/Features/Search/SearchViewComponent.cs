using Generic.Models;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Features.Search
{
    [ViewComponent]
    public class SearchViewComponent : ViewComponent
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SearchViewComponent(ISearchRepository searchRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _searchRepository = searchRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get values from Query String
            string searchValue = null;
            int page = 1;
            int pageSize = 100;
            if(_httpContextAccessor.HttpContext.Request.Query.TryGetValue("searchValue", out StringValues querySearchValue) && querySearchValue.Any())
            {
                searchValue = querySearchValue.FirstOrDefault();
            }
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("page", out StringValues queryPage) && queryPage.Any())
            {
                _ = int.TryParse(queryPage.FirstOrDefault(), out page);
            }
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("pageSize", out StringValues queryPageSize) && queryPageSize.Any())
            {
                _ = int.TryParse(queryPageSize.FirstOrDefault(), out pageSize);
            }

            var model = new SearchViewModel()
            {
                SearchValue = searchValue,
                CurrentPage = page,
                PageSize = pageSize
            };

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                model.SearchResults = await _searchRepository.Search(searchValue, new[] { "SiteSearch_Pages" }, page, pageSize);
            }
            return View("Search", model);
        }
    }

    public record SearchViewModel
    {
        public SearchViewModel()
        {

        }
        public string SearchValue { get; set; }
        public SearchResponse SearchResults { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}
