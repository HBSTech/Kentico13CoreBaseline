namespace Core.Components.ConfigurationHelper
{
    public class ConfigurationHelperViewComponent : ViewComponent
    {
        private readonly IBaselinePageBuilderContext _pageBuilderContext;

        public ConfigurationHelperViewComponent(IBaselinePageBuilderContext pageBuilderContext)
        {
            _pageBuilderContext = pageBuilderContext;
        }

        public IViewComponentResult Invoke(string xInstructions, bool xNeedsAttention = true, ConfigurationHelperMode xMode = ConfigurationHelperMode.Inline, bool xVisible = true)
        {
            if(!_pageBuilderContext.IsEditMode || !xVisible)
            {
                return Content(string.Empty);
            }
            return View("/Components/ConfigurationHelper/ConfigurationHelper.cshtml", new ConfigurationHelperViewModel()
            {
                Mode = xMode,
                NeedsAttention = xNeedsAttention,
                Instructions = xInstructions
            });
        }
    }

    public struct ConfigurationHelperViewModel
    {
        public ConfigurationHelperMode Mode { get; set; }
        public bool NeedsAttention { get; set; }
        public string Instructions { get; set; }

    }

    public enum ConfigurationHelperMode
    {
        ToolTip, Inline
    }
}
