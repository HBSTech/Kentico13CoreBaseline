using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using Generic.Controllers;
using Generic.ViewModels;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CMS.DocumentEngine.Types.Generic.Tab;

[assembly: RegisterPageRoute(TabParent.CLASS_NAME, typeof(TabController), ActionName = "TabParent")]
[assembly: RegisterPageRoute(Tab.CLASS_NAME, typeof(TabController), ActionName = "Tab")]
namespace Generic.Controllers
{
    public class TabController : Controller
    {
        public IPageDataContextRetriever PageDataContextRetriever { get; }
        public IPageRetriever PageRetriever { get; }

        public TabController(IPageDataContextRetriever pageDataContextRetriever,
            IPageRetriever pageRetriever)
        {
            PageDataContextRetriever = pageDataContextRetriever;
            PageRetriever = pageRetriever;
        }

        public ActionResult TabParent()
        {
            var CurrentTabParentPage = PageDataContextRetriever.Retrieve<TabParent>().Page;
            // Get child tabs to render
            var ChildTabs = PageRetriever.Retrieve<Tab>(query =>
                query.Path(CurrentTabParentPage.NodeAliasPath, PathTypeEnum.Children)
            ).Select(x => new TabFields(x));

            TabParentViewModel model = new TabParentViewModel()
            {
                Tabs = ChildTabs
            };

            return View(model);
        }

        public ActionResult Tab()
        {
            return View();
        }
    }
}
