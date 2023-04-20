namespace Core.Services.Implementations
{
    [AutoDependencyInjection]
    public class LinkItemService : ILinkItemService
    {
        public Tuple<string, string> GetLinkBeginningEnd(ILinkItem linkItem, string? title = null, string? cssClass = null)
        {
            bool isButton = linkItem.GetLinkType() == LinkType.Button;
            bool differentTarget = linkItem.GetLinkTarget() != LinkTargetType._self;
            bool hasClass = isButton || cssClass.AsNullOrWhitespaceMaybe().HasValue;
            string beginning = $"<a href=\"{linkItem.GetLinkUrl()}\" {(hasClass ? $"class=\"{(isButton ? "btn btn-primary" : "")} {cssClass}\"" : "")} {(differentTarget ? $"target=\"{linkItem.GetLinkTarget().ToString()}\"" : "")} {(title.AsNullOrWhitespaceMaybe().HasValue ? $"title=\"{title}\"" : "")} >";
            return new Tuple<string, string>(beginning, "</a>");
        }

        public string GetLinkHtml(ILinkItem linkItem, string? title = null, string? cssClass = null)
        {
            var beginningEnd = GetLinkBeginningEnd(linkItem, title, cssClass);
            return $"{beginningEnd.Item1}{linkItem.GetLinkText().GetValueOrDefault(string.Empty)}{beginningEnd.Item2}";
        }
    }
}
