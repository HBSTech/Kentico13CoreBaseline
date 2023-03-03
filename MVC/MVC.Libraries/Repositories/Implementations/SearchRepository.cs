using AutoMapper;
using CMS.Membership;
using CMS.Search;
using CMS.WebAnalytics;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class SearchRepository : ISearchRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserInfoProvider _userInfoProvider;
        private readonly IMapper _mapper;
        private readonly IPagesActivityLogger _pagesActivityLogger;

        public SearchRepository(IHttpContextAccessor httpContextAccessor,
            IUserInfoProvider userInfoProvider,
            IMapper mapper,
            IPagesActivityLogger pagesActivityLogger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userInfoProvider = userInfoProvider;
            _mapper = mapper;
            _pagesActivityLogger = pagesActivityLogger;
        }

        public async Task<SearchResponse> Search(string searchValue, IEnumerable<string> indexes, int page = 1, int pageSize = 100)
        {
            var httpUser = _httpContextAccessor.HttpContext.User;
            var user = await _userInfoProvider.GetAsync(httpUser.Identity.IsAuthenticated ? httpUser.Identity.Name : "public");
            var searchParameters = SearchParameters.PrepareForPages(searchValue, indexes, page, pageSize, user);
            var Search = SearchHelper.Search(searchParameters);
            
            // Log search
            _pagesActivityLogger.LogInternalSearch(searchValue);

            var searchResponse = new SearchResponse()
            {
                Items = Search.Items.Select(x => _mapper.Map<SearchItem>(x)),
                TotalPossible = Search.TotalNumberOfResults,
                HighlightedWords = Search.Highlights,
                HighlightRegex = Search.HighlightRegex
            };
            // Can modify the mapping to add custom logic in the AutoMapperMaps.cs -> SearchItemMapping
            return searchResponse;
        }
    }
}
