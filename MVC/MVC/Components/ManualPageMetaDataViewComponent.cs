using CMS.DocumentEngine;
using Generic.Models;
using Generic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MVC.RepositoryLibrary.Interfaces;
using System.Threading.Tasks;

namespace Generic.Components
{
    [ViewComponent(Name = "ManualPageMetaData")]
    public class ManualPageMetaDataViewComponent : ViewComponent
    {
        public ManualPageMetaDataViewComponent()
        {
            
        }

        /// <summary>
        /// Render Page meta data with a custom meta data item
        /// </summary>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(PageMetaData metaData)
        {
            PageMetaDataViewModel model = new PageMetaDataViewModel()
            {
                Title = metaData.Title,
                Keywords = metaData.Keywords,
                Description = metaData.Description,
                Thumbnail = metaData.Thumbnail
            };
            return View("~/Views/Shared/Components/PageMetaData/Default.cshtml", model);
        }
    }
}
