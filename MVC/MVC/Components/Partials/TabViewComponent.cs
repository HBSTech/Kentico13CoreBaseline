using CMS.DocumentEngine.Types.Generic;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static CMS.DocumentEngine.Types.Generic.Tab;

namespace Generic.Components
{
    [ViewComponent(Name = "TabComponent")]
    public class TabViewComponent : ViewComponent
    {
        public TabViewComponent(IPageDataContextRetriever DataRetriever)
        {
            this.DataRetriever = DataRetriever;
        }

        public IPageDataContextRetriever DataRetriever { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var CurrentTabPage = DataRetriever.Retrieve<Tab>().Page;
            return View("~/Views/Shared/Components/Tab/Default.cshtml",new TabFields(CurrentTabPage));
        }
    }
}
