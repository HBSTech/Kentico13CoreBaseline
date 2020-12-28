using CMS.DocumentEngine;
using Generic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Generic.Components
{
    [ViewComponent(Name = "PageMetaData")]
    public class PageMetaDataViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(TreeNode Page, string ImageUrl = null)
        {
            PageMetaDataViewModel model = new PageMetaDataViewModel()
            {
                Title = Page.DocumentPageTitle,
                Keywords = Page.DocumentPageKeyWords,
                Description = Page.DocumentPageDescription,
                Thumbnail = ImageUrl
            };
            return View(model);
        }
    }
}
