namespace Core.Models
{
    public class GeneralLink : ILinkItem
    {
        public GeneralLink(string linkUrl, LinkTargetType linkTargetType)
        {
            LinkUrl = linkUrl;
            LinkTargetType = linkTargetType;
        }

        public GeneralLink(string linkUrl, LinkTargetType linkTargetType, string linkText)
        {
            LinkUrl = linkUrl;
            LinkTargetType = linkTargetType;
            LinkText = linkText.AsMaybe();
        }

        public string LinkUrl { get; }
        public Maybe<string> LinkText { get; }
        public LinkTargetType LinkTargetType { get; }

        public LinkTargetType GetLinkTarget() => LinkTargetType;

        public Maybe<string> GetLinkText() => LinkText;

        public LinkType GetLinkType() => LinkType.General;

        public string GetLinkUrl() => LinkUrl;
    }
}
