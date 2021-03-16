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
using MVCCaching.Base.Core.Interfaces;

[assembly: RegisterWidget(ImageWidgetViewComponent.IDENTIFIER, typeof(ImageWidgetViewComponent), "Image", propertiesType: typeof(ImageWidgetProperties), Description = "Places an image on the page", IconClass = "icon-picture", AllowCache = true)]

namespace Generic.Widgets
{
    [ViewComponent()]
    public class ImageWidgetViewComponent : ViewComponent
    {
        public const string IDENTIFIER = "Generic.ImageWidget";
        readonly IMediaRepository MediaRepository;
        private readonly ICacheDependenciesStore cacheDependenciesStore;
        private readonly ICacheDependenciesScope cacheDependenciesScope;

        public ImageWidgetViewComponent(IMediaRepository mediaRepository, ICacheDependenciesStore cacheDependenciesStore, ICacheDependenciesScope cacheDependenciesScope)
        {

            MediaRepository = mediaRepository;
            this.cacheDependenciesStore = cacheDependenciesStore;
            this.cacheDependenciesScope = cacheDependenciesScope;
        }
        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<ImageWidgetProperties> widgetProperties)
        {
            cacheDependenciesScope.Begin();

            var Properties = widgetProperties.Properties;
            string ImageUrl;
            if (Properties.UseAttachment)
            {
                if (!string.IsNullOrWhiteSpace(Properties.ImageGuid))
                {
                    cacheDependenciesStore.Store(new string[] { $"attachment|{Guid.Parse(Properties.ImageGuid)}" });
                    ImageUrl = await MediaRepository.GetAttachmentImageAsync(widgetProperties.Page, Guid.Parse(Properties.ImageGuid));
                } else
                {
                    ImageUrl = "";
                }
            }
            else
            {
                if (Properties.ImageUrl != null && Properties.ImageUrl.Any(x => true))
                {
                    cacheDependenciesStore.Store(new string[] { $"mediafile|{Properties.ImageUrl.FirstOrDefault().FileGuid}" });
                    ImageUrl = await MediaRepository.GetMediaFileUrlAsync(Properties.ImageUrl.FirstOrDefault().FileGuid);
                } else
                {
                    ImageUrl = "";
                }
            }

            // Save dependencies
            widgetProperties.CacheDependencies.CacheKeys = cacheDependenciesScope.End();

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