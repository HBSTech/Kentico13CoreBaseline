namespace SectionsSystem.Services
{
    public interface IVisualItemHelper
    {
        /// <summary>
        /// Central spot to run the various switch / logics for Visual Items
        /// </summary>
        /// <param name="visualItemType">either "image" or "icon" currently</param>
        /// <param name="libraryIconCode"></param>
        /// <param name="themeIconCode"></param>
        /// <param name="customIconCode"></param>
        /// <param name="iconAlt"></param>
        /// <param name="imageUrl"></param>
        /// <param name="imageAlt"></param>
        /// <param name="generalLink"></param>
        /// <returns></returns>
        IVisualItem GenerateVisualItem(VisualItemType visualItemType, string iconSource, string libraryIconCode, string themeIconCode, string customIconCode, string iconAlt, string imageUrl, string imageAlt, Maybe<GeneralLink> generalLink);

        IVisualItem GenerateVisualItemImage(string imageUrl, string imageAlt, Maybe<GeneralLink> generalLink);

        IVisualItem GenerateVisualItemIcon(VisualItemType visualItemType, string iconSource, string iconCode, string iconAlt, Maybe<GeneralLink> generalLink);
    }
}
