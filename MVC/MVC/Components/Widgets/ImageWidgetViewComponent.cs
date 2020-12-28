using Generic.Widgets;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.ViewModels;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Generic.InlineEditors;
using System.Threading.Tasks;
using System.Linq;
using System;

[assembly: RegisterWidget(ImageWidgetViewComponent.IDENTIFIER, typeof(ImageWidgetViewComponent), "Image", propertiesType: typeof(ImageWidgetProperties), Description = "Places an image on the page", IconClass = "icon-picture")]

namespace Generic.Widgets
{
    [ViewComponent()]
    public class ImageWidgetViewComponent : ViewComponent
    {
        public const string IDENTIFIER = "Generic.ImageWidget";
        readonly IMediaRepository MediaRepository;

        public ImageWidgetViewComponent(IMediaRepository mediaRepository)
        {

            MediaRepository = mediaRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<ImageWidgetProperties> widgetProperties)
        {
            var Properties = widgetProperties.Properties;
            string ImageUrl;
            if (Properties.UseAttachment)
            {
                if (!string.IsNullOrWhiteSpace(Properties.ImageGuid))
                {
                    ImageUrl = await MediaRepository.GetAttachmentImageAsync(widgetProperties.Page, Guid.Parse(Properties.ImageGuid));
                } else
                {
                    ImageUrl = "";
                }
            }
            else
            {
                ImageUrl = (Properties.ImageUrl != null && Properties.ImageUrl.Count() > 0 ? await MediaRepository.GetMediaFileUrlAsync(Properties.ImageUrl.FirstOrDefault().FileGuid) : "");
            }
            ImageWidgetViewModel model = new ImageWidgetViewModel()
            {
                ImageUrl = ImageUrl,
                Alt = Properties.Alt,
                CssClass = Properties.CssClass,
                ImageType = Properties.UseAttachment ? ImageTypeEnum.Attachment : ImageTypeEnum.MediaFile
            };

            return View(model);
        }
    }
}