using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using RichTextWidget.KX13.Components.Widgets.RichTextWidget;

[assembly: RegisterWidget(RichTextWidgetProperties.IDENTIFIER, "Rich Text (Container)",
    typeof(RichTextWidgetProperties),
    customViewName: "~/Components/Widgets/RichTextWidget/RichTextWidget.cshtml",
    Description = "Text area where the content will be rendered as is as HTML through the widget dialog",
    IconClass = "icon-l-lightbox")]
namespace RichTextWidget.KX13.Components.Widgets.RichTextWidget
{
    public class RichTextWidgetProperties : IWidgetProperties
    {
        public const string IDENTIFIER = "RichTextEditor";
        [EditingComponent(RichTextComponent.IDENTIFIER, Label = "Html Content")]
        public string Html { get; set; } = string.Empty;

        public RichTextWidgetProperties()
        {

        }
    }
}