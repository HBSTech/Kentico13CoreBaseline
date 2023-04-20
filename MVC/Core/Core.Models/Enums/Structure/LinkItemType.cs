namespace Core.Enums
{
    public enum LinkType
    {
        /// <summary>
        /// Just a link, this type is often used if the entire area is to be linked or if a VisualItem has a link.  Class should be of type GeneralLink
        /// </summary>
        General,
        /// <summary>
        /// A text hyperlink. Class should be of TextLink 
        /// </summary>
        Text,
        /// <summary>
        /// A Button link.  Class should be of type ButtonLink
        /// </summary>
        Button
    }
}
