using Kentico.PageBuilder.Web.Mvc;
using Kentico.Forms.Web.Mvc;
using Kentico.Components.Web.Mvc.FormComponents;
using ImageWidget.Components.Widgets.ImageWidget;

//[assembly: RegisterWidget(ImageWidgetViewComponent.IDENTIFIER, typeof(ImageWidgetViewComponent), "Image", propertiesType: typeof(ImageWidgetProperties), Description = "Places an image on the page", IconClass = "icon-picture", AllowCache = true)]
namespace ImageWidget.Components.Widgets.ImageWidget
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

            var properties = widgetProperties.Properties;

            if (properties.ImageUrl.FirstOrMaybe().TryGetValue(out var imageItem))
            {
                _cacheDependenciesScope.Begin();

                var mediaItem = (await _mediaRepository.GetMediaItemAsync(imageItem.FileGuid));
                _cacheDependenciesScope.End();

                if (mediaItem.TryGetValue(out var mediaFile))
                {
                    var model = new ImageWidgetViewModel(
                        imageUrl: mediaFile.MediaUrl,
                        alt: properties.Alt.AsNullOrWhitespaceMaybe().GetValueOrDefault(mediaFile.MediaTitle).GetValueOrDefault(mediaFile.MediaName))
                    {
                        CssClass = properties.CssClass.AsNullOrWhitespaceMaybe()
                    };
                    return View("/Components/Widgets/ImageWidget/ImageWidget.cshtml", model);
                }

            }

            return View("/Components/Widgets/ImageWidget/ImageWidget.cshtml", new ImageWidgetViewModel());
        }
    }

    public record ImageWidgetProperties : IWidgetProperties
    {
        [EditingComponent(MediaFilesSelector.IDENTIFIER, Label = "Media relative link", Order = 2)]
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.MaxFilesLimit), 1)]
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.AllowedExtensions), ".gif;.png;.jpg;.jpeg")]
        public IEnumerable<MediaFilesSelectorItem> ImageUrl { get; set; } = Array.Empty<MediaFilesSelectorItem>();

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Image Alt", Order = 3)]
        public string? Alt { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "CSS Class", Order = 4)]
        public string? CssClass { get; set; }

    }

    public record ImageWidgetViewModel
    {
        public ImageWidgetViewModel(string imageUrl, string alt)
        {
            ImageUrl = imageUrl;
            Alt = alt;
        }
        // If no item selected
        public ImageWidgetViewModel()
        {

        }

        /// <summary>
        /// Image.
        /// </summary>
        public Maybe<string> ImageUrl { get; set; }

        public Maybe<string> Alt { get; set; }

        public Maybe<string> CssClass { get; set; }
    }

}