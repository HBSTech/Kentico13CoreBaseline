using Microsoft.AspNetCore.Mvc;

namespace Search.Features.Search
{
    public class SearchController : Controller
    {
        public SearchController()
        {
        }
        
        /// <summary>
        /// Fall back, should really use page templates
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("/Search")]
        public ActionResult Index()
        {
            return View("/Features/Search/SearchManual.cshtml");
        }
    }
}