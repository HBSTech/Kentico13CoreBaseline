using CMS.DocumentEngine.Types.Generic;
using Generic.Controllers;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Kentico.Content.Web.Mvc;
using System.Threading.Tasks;

[assembly: RegisterPageRoute(Home.CLASS_NAME, typeof(HomeController))]
namespace Generic.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPageDataContextRetriever PageDataContextRetriever;

        public HomeController(IPageDataContextRetriever pageDataContextRetriever)
        {
            PageDataContextRetriever = pageDataContextRetriever;
        }

        public async Task<ActionResult> Index()
        {
            var Retriever = PageDataContextRetriever.Retrieve<Home>();
           
            if (Retriever.Page == null)
            {
                return StatusCode(404);
            }
            // Use template if it has one.
            return View(Retriever.Page);
        }
    }
}