namespace Core.Models
{
    public class ContentItem : IContentItem
    {
        public ContentItem()
        {

        }

        public Maybe<string> Header { get; set; }
        public Maybe<string> SubHeader { get; set; }
        public Maybe<string> HtmlContent { get; set; }
    }
}
