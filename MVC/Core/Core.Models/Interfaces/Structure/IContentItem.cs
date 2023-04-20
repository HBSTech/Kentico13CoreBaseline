namespace Core.Interfaces
{
    public interface IContentItem
    {
        Maybe<string> Header { get; set; }
        Maybe<string> HtmlContent { get; set; }
        Maybe<string> SubHeader { get; set; }
    }
}