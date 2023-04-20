namespace Core.Interfaces
{
    public interface ILinkItem
    {
        LinkType GetLinkType();
        LinkTargetType GetLinkTarget();
        string GetLinkUrl();
        Maybe<string> GetLinkText();
    }
}
