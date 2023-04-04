using XperienceCommunity.PageBuilderUtilities;

namespace Core.Repositories.Implementation
{
    public class BaselinePageBuilderContext : IBaselinePageBuilderContext
    {
        private readonly IPageBuilderContext _pageBuilderContext;

        public BaselinePageBuilderContext(IPageBuilderContext pageBuilderContext)
        {
            _pageBuilderContext = pageBuilderContext;
        }

        public bool IsPreviewMode => _pageBuilderContext.IsPreviewMode;

        public bool IsLiveMode => _pageBuilderContext.IsLiveMode;

        public bool IsLivePreviewMode => _pageBuilderContext.IsLivePreviewMode;

        public bool IsEditMode => _pageBuilderContext.IsEditMode;

        public BaselinePageBuilderMode Mode => _pageBuilderContext.Mode switch
        {
            PageBuilderMode.Edit => BaselinePageBuilderMode.Edit,
            PageBuilderMode.Live => BaselinePageBuilderMode.Live,
            PageBuilderMode.LivePreview => BaselinePageBuilderMode.LivePreview,
            _ => BaselinePageBuilderMode.Live
        };

        public string ModeName() => _pageBuilderContext.ModeName();
    }
}
