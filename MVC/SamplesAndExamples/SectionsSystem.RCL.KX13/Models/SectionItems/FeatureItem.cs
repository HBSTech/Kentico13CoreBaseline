namespace SectionsSystem.Models.SectionItems
{
    public class FeatureItem
    {
        public FeatureItem(ContentItem content, MediaItem image, List<ILinkItem> links)
        {
            Content = content;
            Image = image;
            Links = links;
        }

        public ContentItem Content { get; set; }
        public MediaItem Image { get; set; }
        public List<ILinkItem> Links { get; set; }

    }
}
