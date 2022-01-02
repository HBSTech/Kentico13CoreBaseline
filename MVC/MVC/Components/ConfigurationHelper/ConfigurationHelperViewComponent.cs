using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XperienceCommunity.PageBuilderUtilities;

namespace Generic.Components.ConfigurationHelper
{
    public class ConfigurationHelperViewComponent : ViewComponent
    {
        private readonly IPageBuilderContext _pageBuilderContext;

        public ConfigurationHelperViewComponent(IPageBuilderContext pageBuilderContext)
        {
            _pageBuilderContext = pageBuilderContext;
        }

        public IViewComponentResult Invoke(bool visible, ConfigurationHelperMode mode, bool needsAttention, string instructions)
        {
            if(!_pageBuilderContext.IsEditMode || !visible)
            {
                return Content(string.Empty);
            }
            return View("ConfigurationHelper", new ConfigurationHelperViewModel()
            {
                Mode = mode,
                NeedsAttention = needsAttention,
                Instructions = instructions
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
