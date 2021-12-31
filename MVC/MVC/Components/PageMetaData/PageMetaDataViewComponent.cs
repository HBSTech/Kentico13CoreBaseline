using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Generic.Components.PageMetaData
{
    [ViewComponent]
    public class PageMetaDataViewComponent : ViewComponent
    {
        private readonly IMetaDataRepository _metaDataRepository;

        public PageMetaDataViewComponent(IMetaDataRepository metaDataRepository)
        {
            _metaDataRepository = metaDataRepository;
        }


        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var metaData = await _metaDataRepository.GetMetaDataAsync();
            var model = new PageMetaDataViewModel()
            {
                Title = metaData.Title,
                Keywords = metaData.Keywords,
                Description = metaData.Description,
                Thumbnail = metaData.Thumbnail
            };
            return View("PageMetaData", model);
        }
    }
}
