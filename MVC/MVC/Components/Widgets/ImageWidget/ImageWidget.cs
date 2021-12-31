using Generic.Repositories.Interfaces;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System;
using MVCCaching.Base.Core.Interfaces;
using Kentico.Forms.Web.Mvc;
using Kentico.Components.Web.Mvc.FormComponents;
using System.Collections.Generic;
using Generic.Components.Widgets.ImageWidget;
using Generic.Components.InlineEditors.ImageUploaderEditor;

[assembly: RegisterWidget(ImageWidgetViewComponent.IDENTIFIER, typeof(ImageWidgetViewComponent), "Image", propertiesType: typeof(ImageWidgetProperties), Description = "Places an image on the page", IconClass = "icon-picture", AllowCache = true)]
namespace Generic.Components.Widgets.ImageWidget
{
    [ViewComponent]
    public class ImageWidgetViewComponent : ViewComponent
    {
        public const string IDENTIFIER = "Generic.ImageWidget";
        private readonly IMediaRepository _mediaRepository;
        private readonly ICacheDependenciesScope _cacheDependenciesScope;

        public ImageWidgetViewComponent(IMediaRepository mediaRepository,
            ICacheDependenciesScope cacheDependenciesScope)
        {
            _mediaRepository = mediaRepository;
            _cacheDependenciesScope = cacheDependenciesScope;
        }
        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<ImageWidgetProperties> widgetProperties)
        {
            _cacheDependenciesScope.Begin();

            var Properties = widgetProperties.Properties;
            string ImageUrl;
            if (Properties.UseAttachment)
            {
                if (!string.IsNullOrWhiteSpace(Properties.ImageGuid))
                {
                    ImageUrl = (await _mediaRepository.GetPageAttachmentAsync(Guid.Parse(Properties.ImageGuid), widgetProperties.Page.DocumentID)).AttachmentUrl;
                } else
                {
                    ImageUrl = "";
                }
            }
            else
            {
                if (Properties.ImageUrl != null && Properties.ImageUrl.Any(x => true))
                {
                    ImageUrl = (await _mediaRepository.GetMediaItemAsync(Properties.ImageUrl.FirstOrDefault().FileGuid)).MediaUrl;
                } else
                {
                    ImageUrl = "";
                }
            }

            // Save dependencies
            widgetProperties.CacheDependencies.CacheKeys = _cacheDependenciesScope.End();

            var model = new ImageWidgetViewModel()
            {
                ImageUrl = ImageUrl,
                Alt = Properties.Alt,
                CssClass = Properties.CssClass,
                ImageType = Properties.UseAttachment ? ImageTypeEnum.Attachment : ImageTypeEnum.MediaFile
            };

            return View("ImageWidget", model);
        }
    }

    public record ImageWidgetProperties : IWidgetProperties
    {
        /// <summary>
        /// Guid of an image to be displayed.
        /// </summary>
        public string ImageGuid { get; set; }

        [EditingComponent(CheckBoxComponent.IDENTIFIER, Label = "Use Attachment", Tooltip = "Uncheck if you wish to use the below media library path.", Order = 1)]
        public bool UseAttachment { get; set; } = true;

        [EditingComponent(MediaFilesSelector.IDENTIFIER, Label = "Media relative link", Order = 2)]
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.MaxFilesLimit), 1)]
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.AllowedExtensions), ".gif;.png;.jpg;.jpeg")]
        [VisibilityCondition(nameof(UseAttachment), ComparisonTypeEnum.IsFalse)]
        public IEnumerable<MediaFilesSelectorItem> ImageUrl { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Image Alt", Order = 3)]
        public string Alt { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "CSS Class", Order = 4)]
        public string CssClass { get; set; }
    }

    public record ImageWidgetViewModel
    {
        public Guid ImageGuid { get; set; }
        /// <summary>
        /// Image.
        /// </summary>
        public string ImageUrl { get; set; }

        public string Alt { get; set; }

        public string CssClass { get; set; }
        public ImageTypeEnum ImageType { get; set; }
    }
   
}