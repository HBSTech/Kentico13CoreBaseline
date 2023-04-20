using Core.Components.ConfigurationHelper;

namespace Core.Features.PageBuilderError
{
    public record PageBuilderErrorViewModel
    {
        public PageBuilderErrorViewModel(string message, bool inline, bool isError)
        {
            Message = message;
            if (inline)
            {
                Mode = ConfigurationHelperMode.Inline;
            }
            else
            {
                Mode = ConfigurationHelperMode.ToolTip;
            }
            NeedsAttention = isError;
        }
        public string Message { get; set; }
        public ConfigurationHelperMode Mode { get; set; }
        public bool NeedsAttention { get; }
    }
}
