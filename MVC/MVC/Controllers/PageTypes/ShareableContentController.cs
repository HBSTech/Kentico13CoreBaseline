using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Generic;
using CMS.DocumentEngine.Types.Generic;

[assembly: RegisterPageRoute(ShareableContent.CLASS_NAME, typeof(ShareableContentController))]
namespace Generic
{
    public class ShareableContentController : Controller
    {
        public ShareableContentController(IPageRetriever pageRetriever,
            IPageDataContextRetriever dataRetriever
            )
        {
            PageRetriever = pageRetriever;
            DataRetriever = dataRetriever;
        }

        public IPageDataContextRetriever DataRetriever { get; }
        public IPageRetriever PageRetriever { get; }

        public ActionResult Index()
        {
            return View();
        }
    }
}
