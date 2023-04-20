namespace SectionsSystem.Services.Implementation
{
    [AutoDependencyInjection]
    public class VisualItemHelper : IVisualItemHelper
    {
        private readonly ICoreStringToEnumParser _coreStringToEnumParser;

        public VisualItemHelper(ICoreStringToEnumParser coreStringToEnumParser)
        {
            _coreStringToEnumParser = coreStringToEnumParser;
        }

        public IVisualItem GenerateVisualItem(VisualItemType visualItemType, string iconSource, string libraryIconCode, string themeIconCode, string customIconCode, string iconAlt, string imageUrl, string imageAlt, Maybe<GeneralLink> generalLink)
        {

            switch (visualItemType)
            {
                default:
                case VisualItemType.Image:
                    if (generalLink.HasValue)
                    {
                        if (imageAlt.AsNullOrWhitespaceMaybe().TryGetValue(out var imageAltVal))
                        {
                            return new ImageItem(imageUrl, imageAltVal, generalLink.Value);
                        }
                        else
                        {
                            return new ImageItem(imageUrl, generalLink.Value);
                        }
                    }
                    else
                    {
                        if (imageAlt.AsNullOrWhitespaceMaybe().TryGetValue(out var imageAltVal))
                        {
                            return new ImageItem(imageUrl, imageAltVal);
                        }
                        else
                        {
                            return new ImageItem(imageUrl);
                        }
                    }
                case VisualItemType.Icon:
                    string iconCode = string.Empty;
                    var iconSourceVal = _coreStringToEnumParser.StringToIconSource(iconSource);

                    switch (iconSourceVal)
                    {
                        case IconSource.IconLibrary:
                            iconCode = libraryIconCode;
                            break;
                        case IconSource.ThemeIcons:
                            iconCode = themeIconCode;
                            break;
                        case IconSource.Custom:
                            iconCode = customIconCode;
                            break;
                    }

                    if (generalLink.HasValue)
                    {
                        return new IconItem(iconSourceVal, iconCode, iconAlt.AsNullOrWhitespaceMaybe().GetValueOrDefault($"icon {iconCode}"), generalLink.Value);
                    }
                    else
                    {
                        return new IconItem(iconSourceVal, iconCode, iconAlt.AsNullOrWhitespaceMaybe().GetValueOrDefault($"icon {iconCode}"));
                    }
            }
        }

        public IVisualItem GenerateVisualItemIcon(VisualItemType visualItemType, string iconSource, string iconCode, string iconAlt, Maybe<GeneralLink> generalLink)
        {
            var iconSourceVal = _coreStringToEnumParser.StringToIconSource(iconSource);

            if (generalLink.HasValue)
            {
                return new IconItem(iconSourceVal, iconCode, iconAlt.AsNullOrWhitespaceMaybe().GetValueOrDefault($"icon {iconCode}"), generalLink.Value);
            }
            else
            {
                return new IconItem(iconSourceVal, iconCode, iconAlt.AsNullOrWhitespaceMaybe().GetValueOrDefault($"icon {iconCode}"));
            }
        }

        public IVisualItem GenerateVisualItemImage(string imageUrl, string imageAlt, Maybe<GeneralLink> generalLink)
        {
            if (generalLink.HasValue)
            {
                if (imageAlt.AsNullOrWhitespaceMaybe().TryGetValue(out var imageAltVal))
                {
                    return new ImageItem(imageUrl, imageAltVal, generalLink.Value);
                }
                else
                {
                    return new ImageItem(imageUrl, generalLink.Value);
                }
            }
            else
            {
                if (imageAlt.AsNullOrWhitespaceMaybe().TryGetValue(out var imageAltVal))
                {
                    return new ImageItem(imageUrl, imageAltVal);
                }
                else
                {
                    return new ImageItem(imageUrl);
                }
            }
        }
    }
}
