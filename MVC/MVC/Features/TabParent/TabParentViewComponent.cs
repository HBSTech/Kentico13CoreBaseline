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

        public TabParentViewComponent(ITabRepository tabRepository)
        {
            _tabRepository = tabRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(PageIdentity<TabParentItem> page)
        {
            var model = new TabParentViewModel();
            if (page == null)
            {
                model = new TabParentViewModel()
                {
                    Name = "Unknown",
                    Tabs = new List<TabItem>()
                };
            }
            else
            {
                model.Name = page.Data.Name;
                model.Tabs = await _tabRepository.GetTabsAsync(page.Path);
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
