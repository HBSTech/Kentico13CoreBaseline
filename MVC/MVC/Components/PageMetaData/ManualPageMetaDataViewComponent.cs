using Microsoft.AspNetCore.Mvc;

namespace Generic.Components.PageMetaData
{
    [ViewComponent]
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
        public IViewComponentResult Invoke(Models.PageMetaData metaData)
        {
            var model = new PageMetaDataViewModel()
            {
                Title = metaData.Title,
                Keywords = metaData.Keywords,
                Description = metaData.Description,
                Thumbnail = metaData.Thumbnail
            };
            return View("~/Components/PageMetaData/PageMetaData.cshtml", model);
        }
    }
   
}
