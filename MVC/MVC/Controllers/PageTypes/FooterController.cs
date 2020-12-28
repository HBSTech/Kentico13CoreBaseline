using CMS.DocumentEngine.Types.Generic;
using Generic.Controllers;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[assembly: RegisterPageRoute(Footer.CLASS_NAME, typeof(FooterController))]
namespace Generic.Controllers
{
    public class FooterController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
