
namespace Core.Services.Implementations
{
    [AutoDependencyInjection]
    public class CoreStringToEnumParser : ICoreStringToEnumParser
    {
        public LinkTargetType StringToLinkTargetType(string value)
        {
            return value.ToLower() switch
            {
                "_blank" => LinkTargetType._blank,
                "_parent" => LinkTargetType._parent,
                "_self" => LinkTargetType._self,
                _ => LinkTargetType._self
            };
        }

        public TextAlignment StringToTextAlignment(string value)
        {
            return value.ToLower() switch
            {
                "left" => TextAlignment.Start,
                "start" => TextAlignment.Start,
                "center" => TextAlignment.Center,
                "end" => TextAlignment.End,
                "right" => TextAlignment.End,
                _ => TextAlignment.Start
            };
        }

        public IconSource StringToIconSource(string value)
        {
            return value.ToLower() switch
            {
                "themeicons" => IconSource.ThemeIcons,
                "iconlibrary" => IconSource.IconLibrary,
                "custom" => IconSource.Custom,
                _ => IconSource.None
            };
        }

        public VisualItemType StringToVisualItemType(string value)
        {
            return value.ToLower() switch
            {
                "icon" => VisualItemType.Icon,
                "image" => VisualItemType.Image,
                _ => VisualItemType.Image
            };
        }
    }
}
