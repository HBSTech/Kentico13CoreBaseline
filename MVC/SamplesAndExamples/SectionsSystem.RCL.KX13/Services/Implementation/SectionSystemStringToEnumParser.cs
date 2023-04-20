using SectionsSystem.Enums;

namespace SectionsSystem.Services.Implementation
{
    [AutoDependencyInjection]
    public class SectionsSystemStringToEnumParser : ISectionsSystemStringToEnumParser
    {
      

        public ColorTheme StringToColorTheme(string value)
        {
            return value.ToLower() switch
            {
                "white" => ColorTheme.White,
                "black" => ColorTheme.Black,
                "red" => ColorTheme.Red,
                "orange" => ColorTheme.Orange,
                "yellow" => ColorTheme.Yellow,
                "green" => ColorTheme.Green,
                "blue" => ColorTheme.Blue,
                "indigo" => ColorTheme.Indigo,
                "violet" => ColorTheme.Violet,
                        _ => ColorTheme.None
            };
        }

        public ShadeType StringToShadeType(string value)
        {
            return value.ToLower() switch
            {
                "none" => ShadeType.None,
                "light" => ShadeType.Light,
                "dark" => ShadeType.Dark,
                _ => ShadeType.None
            };
        }

        public ContrastType StringToContrastType(string value)
        {
            return value.ToLower() switch
            {
                "none" => ContrastType.None,
                "overtext" => ContrastType.OverText,
                "oversection" => ContrastType.OverSection,
                _ => ContrastType.None
            };
        }
    }
}
