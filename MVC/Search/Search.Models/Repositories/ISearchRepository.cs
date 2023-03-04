namespace Search.Repositories
{
    public interface ISearchRepository
    {
        /// <summary>
        /// Searches the given indexes and returns the results
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="indexes"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<SearchResponse> Search(string searchValue, IEnumerable<string> indexes, int page = 1, int pageSize = 100);
    }
}
