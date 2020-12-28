using CMS.DocumentEngine.Types.Generic;
using Generic.Controllers;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[assembly: RegisterPageRoute(Header.CLASS_NAME, typeof(HeaderController))]
namespace Generic.Controllers
{
    public class HeaderController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
