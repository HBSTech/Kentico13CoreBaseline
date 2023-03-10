using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using StaticTextWidget.Components.Widgets.StaticText;

[assembly: RegisterWidget(StaticTextProperties.IDENTIFIER,
    "Static Text", 
    typeof(StaticTextProperties),
    customViewName: "~/Components/Widgets/StaticText/StaticTextWidget.cshtml",
    IconClass = "icon-xml-tag")]

namespace StaticTextWidget.Components.Widgets.StaticText
{
    public class StaticTextProperties : IWidgetProperties, IComponentProperties
    {
        public const string IDENTIFIER = "HBS_StaticTextConainerizedWidget";

        public StaticTextProperties()
        {

        }

        [EditingComponent(TextAreaComponent.IDENTIFIER, Order = 0, Label = "Text")]
        public string Text { get; set; } = string.Empty;
    }
}
