using DocumentFormat.OpenXml.EMMA;

namespace Core.Components.PageMetaData
{
    [ViewComponent]
    public class PageMetaDataViewComponent : ViewComponent
    {
        private readonly IMetaDataRepository _metaDataRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PageMetaDataViewComponent(IMetaDataRepository metaDataRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _metaDataRepository = metaDataRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(int documentId = -1)
        {
            if(_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext) && httpContext.Items.TryGetValue("ManualMetaDataAdded", out var manualAdded))
            {
                // Manual page meta data added, so don't do automatic.
                return Content(string.Empty);
            }

            var metaData = documentId > 0 ? await _metaDataRepository.GetMetaDataAsync(documentId) : await _metaDataRepository.GetMetaDataAsync(documentId);
            if (metaData.TryGetValue(out var metaDataVal))
            {
                var model = new PageMetaDataViewModel()
                {
                    Title = metaDataVal.Title,
                    Keywords = metaDataVal.Keywords,
                    Description = metaDataVal.Description,
                    Thumbnail = metaDataVal.Thumbnail
                };
                return View("/Components/PageMetaData/PageMetaData.cshtml", model);
            }
            else
            {
                return View("/Components/PageMetaData/PageMetaData.cshtml", new PageMetaDataViewModel());
            }
        }
    }
}
