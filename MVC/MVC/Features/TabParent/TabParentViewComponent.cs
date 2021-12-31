using Microsoft.AspNetCore.Mvc;
using Generic.Models;
using System.Collections.Generic;
using Generic.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Generic.Features.TabParent
{
    [ViewComponent]
    public class TabParentViewComponent : ViewComponent
    {
        private readonly ITabRepository _tabRepository;
        private readonly IPageContextRepository _pageContextRepository;

        public TabParentViewComponent(ITabRepository tabRepository,
            IPageContextRepository pageContextRepository)
        {
            _tabRepository = tabRepository;
            _pageContextRepository = pageContextRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int nodeId)
        {
            var parentPage = await _pageContextRepository.GetPageByNodeAsync(nodeId);
            var model = new TabParentViewModel();
            if(parentPage == null)
            {
                model = new TabParentViewModel()
                {
                    Name = "Unknown",
                    Tabs = new List<TabItem>()
                };
            } else {
                model.Name = (await _tabRepository.GetTabParentAsync(nodeId)).Name;
                model.Tabs = await _tabRepository.GetTabsAsync(parentPage.Path);
            }
            return View("TabParent", model);
        }
    }

    public record TabParentViewModel
    {
        public IEnumerable<TabItem> Tabs { get; internal set; }
        public string Name { get; internal set; }
    }
}
