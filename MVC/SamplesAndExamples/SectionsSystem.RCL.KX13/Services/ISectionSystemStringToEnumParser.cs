using SectionsSystem.Enums;

namespace SectionsSystem.Services
{
    public interface ISectionsSystemStringToEnumParser
    {
        /// <summary>
        /// Convert a string to the matching Shade Type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ShadeType StringToShadeType(string value);

        /// <summary>
        /// Convert a string to the matching Contrast Type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ContrastType StringToContrastType(string value);

        /// <summary>
        /// Convert a string to the matching Color Theme
        /// </summary>
        /// <param name="value">the string</param>
        /// <returns>The Color theme, none if not found</returns>
        ColorTheme StringToColorTheme(string value);
    }
}
