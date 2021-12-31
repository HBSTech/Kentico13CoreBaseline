using Generic.Models;
using MVCCaching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface ISearchRepository : IRepository
    {
        Task<SearchResponse> Search(string searchValue, IEnumerable<string> indexes, int page = 1, int pageSize = 100);
    }
}
