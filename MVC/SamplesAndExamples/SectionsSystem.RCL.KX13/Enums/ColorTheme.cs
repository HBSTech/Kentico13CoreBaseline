using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Enums
{
    public enum ColorTheme
    {
        // If adjusting, also adjust StringToEnumParser.StringToColorTheme
        None,
        White,
        Black,
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet
    }



    public static class ColorThemeExtensions
    {
        public static string ToStringCss(this ColorTheme theme) => theme.ToString().ToLower();
        public static string ToStringCss(this Maybe<ColorTheme> theme) => theme.GetValueOrDefault(ColorTheme.None).ToString().ToLower();
    }
}
