using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using PageBuilderContainers;
using Generic.Widgets;
[assembly: RegisterWidget(StaticTextContainerizedWidgetProperties.IDENTIFIER, "Static Text", typeof(StaticTextContainerizedWidgetProperties), customViewName: "~/Views/Shared/Widgets/_StaticTextContainerizedWidget.cshtml", IconClass = "icon-xml-tag")]
namespace Generic.Widgets
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
