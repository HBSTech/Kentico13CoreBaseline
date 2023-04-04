using ImageWidget.Components.Widgets.ImageWidget;
using Kentico.PageBuilder.Web.Mvc;

// Image Widget
[assembly: RegisterWidget(ImageWidgetViewComponent.IDENTIFIER, typeof(ImageWidgetViewComponent), "Image", propertiesType: typeof(ImageWidgetProperties), Description = "Places an image on the page", IconClass = "icon-picture", AllowCache = true)]