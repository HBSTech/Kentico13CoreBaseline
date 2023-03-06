using CMS.DocumentEngine;
using CMS.Membership;
using CMS.Search;
using CMS.WebAnalytics;
using Core.Extensions;
using Microsoft.AspNetCore.Http;
using MVCCaching;

namespace Search.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class SearchRepository : ISearchRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserInfoProvider _userInfoProvider;
        private readonly IPagesActivityLogger _pagesActivityLogger;

        public SearchRepository(IHttpContextAccessor httpContextAccessor,
            IUserInfoProvider userInfoProvider,
            IPagesActivityLogger pagesActivityLogger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userInfoProvider = userInfoProvider;
            _pagesActivityLogger = pagesActivityLogger;
        }

        public async Task<SearchResponse> Search(string searchValue, IEnumerable<string> indexes, int page = 1, int pageSize = 100)
        {
            if (_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext)
                    && httpContext.User != null
                    && httpContext.User.Identity.AsMaybe().TryGetValue(out var identity))
            {
                var user = await _userInfoProvider.GetAsync(identity.IsAuthenticated ? identity.Name : "public");
                var searchParameters = SearchParameters.PrepareForPages(searchValue, indexes, page, pageSize, user);
                var Search = SearchHelper.Search(searchParameters);

                // Log search
                _pagesActivityLogger.LogInternalSearch(searchValue);

                var searchResponse = new SearchResponse()
                {
                    Items = Search.Items.Select(x =>
                    {
                        var resultItem = new SearchItem(
                            documentExtensions: x.DocumentExtensions,
                            image: x.Image,
                            content: x.Content,
                            created: x.Created,
                            title: x.Title,
                            index: x.Index,
                            maxScore: x.MaxScore,
                            position: x.Position,
                            score: x.Score,
                            type: x.Type,
                            id: x.Id,
                            absScore: x.AbsScore
                            );
                        if (x.Data is TreeNode page)
                        {
                            resultItem.IsPage = true;
                            resultItem.PageUrl = page.ToPageIdentity().RelativeUrl;
                        }
                        return resultItem;
                    }),
                    TotalPossible = Search.TotalNumberOfResults,
                    HighlightedWords = Search.Highlights,
                    HighlightRegex = Search.HighlightRegex
                };
                return searchResponse;
            }
            return new SearchResponse();
        }
    }
}
