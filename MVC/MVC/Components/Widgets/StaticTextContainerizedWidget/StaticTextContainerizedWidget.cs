using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using PageBuilderContainers;
using Generic.Components.Widgets.StaticTextContainerizedWidget;

[assembly: RegisterWidget(StaticTextContainerizedWidgetProperties.IDENTIFIER,
    "Static Text", 
    typeof(StaticTextContainerizedWidgetProperties),
    customViewName: "~/Components/Widgets/StaticTextContainerizedWidget/StaticTextContainerizedWidget.cshtml",
    IconClass = "icon-xml-tag")]

namespace Generic.Components.Widgets.StaticTextContainerizedWidget
{
    public class StaticTextContainerizedWidgetProperties : PageBuilderWidgetProperties, IWidgetProperties, IComponentProperties
    {
        public const string IDENTIFIER = "HBS_StaticTextConainerizedWidget";

        public StaticTextContainerizedWidgetProperties()
        {

        }

        [EditingComponent(TextAreaComponent.IDENTIFIER, Order = 0, Label = "Text")]
        public string Text { get; set; }
    }
}
