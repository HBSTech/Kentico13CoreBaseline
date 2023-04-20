namespace Core.Models
{
    public class ImageItem : IVisualItem
    {
        public ImageItem(string imageUrl, string imageAlt)
        {
            ImageUrl = imageUrl;
            ImageAlt = imageAlt;
        }
        public ImageItem(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public ImageItem(string imageUrl, string imageAlt, GeneralLink link)
        {
            ImageUrl = imageUrl;
            ImageAlt = imageAlt;
            Link = link;
        }
        public ImageItem(string imageUrl, GeneralLink link)
        {
            ImageUrl = imageUrl;
            Link = link;
        }

        public string ImageUrl { get; }
        public Maybe<string> ImageAlt { get; }
        public Maybe<GeneralLink> Link { get; }

        public VisualItemType GetVisualType() => VisualItemType.Image;
        public Maybe<GeneralLink> GetLink() => Link;
    }
}
