using Core.Services;

namespace Generic.Components.ImportModelState
{
    /// <summary>
    /// If using PageTemplate View Component with a POST action, the Controller's POST should have the [ExportModelState] Attribute
    /// which will cause it to store the ModelSTate in the TempData, then redirect to your Page Template URL.
    /// This View component will then hydrate the ModelState from the TempData
    /// </summary>
    [ViewComponent]
    public class ImportModelStateViewComponent : ViewComponent
    {
        private readonly IModelStateService _modelStateService;

        public ImportModelStateViewComponent(IModelStateService modelStateService)
        {
            _modelStateService = modelStateService;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            _modelStateService.MergeModelState(ModelState, TempData);

            return Content(string.Empty);
        }
    }
}
