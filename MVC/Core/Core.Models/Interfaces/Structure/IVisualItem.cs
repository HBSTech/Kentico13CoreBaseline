namespace Core.Interfaces
{
    /// <summary>
    /// Represents some sort of visual element, such as an Image, Icon, or similar.
    /// </summary>
    public interface IVisualItem
    {
        VisualItemType GetVisualType();

        Maybe<GeneralLink> GetLink();
    }
}
