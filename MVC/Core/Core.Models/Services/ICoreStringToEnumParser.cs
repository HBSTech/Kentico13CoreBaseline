namespace Core.Services
{
    public interface ICoreStringToEnumParser
    {
        IconSource StringToIconSource(string value);
        LinkTargetType StringToLinkTargetType(string value);
        TextAlignment StringToTextAlignment(string value);
        VisualItemType StringToVisualItemType(string value);
    }
}