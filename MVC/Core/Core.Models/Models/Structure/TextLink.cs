namespace Core.Models
{
    public class TextLink : ILinkItem
    {
        public TextLink(string linkUrl, LinkTargetType linkTargetType, string text)
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

        public LinkType GetLinkType() => LinkType.Text;
        public string GetLinkUrl() => LinkUrl;
    }
}
