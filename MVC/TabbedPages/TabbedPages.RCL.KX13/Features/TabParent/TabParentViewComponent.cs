using Core.Models;
using TabbedPages.Models;
using TabbedPages.Repositories;

namespace TabbedPages.Features.TabParent
{
    [ViewComponent]
    public class TabParentViewComponent : ViewComponent
    {
        private readonly ITabRepository _tabRepository;

        public TabParentViewComponent(ITabRepository tabRepository)
        {
            _tabRepository = tabRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(PageIdentity page)
        {
            var model = new TabParentViewModel(
                name: page.Name,
                tabs: await _tabRepository.GetTabsAsync(page.NodeIdentity)
                );
            return View("/Features/TabParent/TabParent.cshtml", model);
        }
    }

    public record TabParentViewModel
    {
        public TabParentViewModel(IEnumerable<TabItem> tabs, string name)
        {
            Tabs = tabs;
            Name = name;
        }

        public IEnumerable<TabItem> Tabs { get; set; }
        public string Name { get; set; }
    }
}
