namespace Core.Models
{
    public class IconItem : IVisualItem
    {
        public IconItem(IconSource iconSource, string iconCode, string altText)
        {
            IconSource = iconSource;
            IconCode = iconCode;
            AltText = altText;
        }

        public IconItem(IconSource iconSource, string iconCode, string altText, GeneralLink link)
        {
            IconSource = iconSource;
            IconCode = iconCode;
            Link = link;
            AltText = altText;
        }

        public IconSource IconSource { get; }
        public string IconCode { get; }
        public Maybe<GeneralLink> Link { get; }
        public string AltText { get; }

        public Maybe<GeneralLink> GetLink() => Link;
        public VisualItemType GetVisualType() => VisualItemType.Icon;
    }
}
