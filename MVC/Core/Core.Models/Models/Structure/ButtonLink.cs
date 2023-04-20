namespace Core.Models
{
    public class ButtonLink : ILinkItem
    {
        public ButtonLink(string linkUrl, LinkTargetType linkTargetType, string text)
        {
            LinkUrl = linkUrl;
            LinkTargetType = linkTargetType;
            Text = text;
        }

        public string LinkUrl { get; }

        public LinkTargetType LinkTargetType { get; }

        public string Text { get; }

        public LinkTargetType GetLinkTarget() => LinkTargetType;

        public Maybe<string> GetLinkText() => Text;

        public LinkType GetLinkType() => LinkType.Button;

        public string GetLinkUrl() => LinkUrl;
    }
}
