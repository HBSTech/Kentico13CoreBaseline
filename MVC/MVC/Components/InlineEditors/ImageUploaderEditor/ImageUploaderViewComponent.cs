using CMS.DocumentEngine;
using Generic.Repositories.Interfaces;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Threading.Tasks;

namespace Generic.Components.InlineEditors.ImageUploaderEditor
{
    [ViewComponent(Name = "ImageUploader")]
    public class ImageUploaderViewComponent : ViewComponent
    {

        private readonly IPageDataContextRetriever _pageDataContextRetriever;
        private readonly ISiteSettingsRepository _siteSettingsRepository;

        public ImageUploaderViewComponent(IPageDataContextRetriever pageDataContextRetriever,
            ISiteSettingsRepository siteSettingsRepository)
        {
            _pageDataContextRetriever = pageDataContextRetriever;
            _siteSettingsRepository = siteSettingsRepository;
        }


        public async Task<IViewComponentResult> InvokeAsync(string propertyName, bool hasImage, ImageTypeEnum imageType, bool useAbsolutePosition, 
            PanelPositionEnum messagePosition)
        {
            string mediaLibrary = await _siteSettingsRepository.GetImageUploadMediaLibraryAsync();
            var model = new ImageUploaderEditorViewModel
            {
                PropertyName = propertyName,
                HasImage = hasImage,
                UseAbsolutePosition = useAbsolutePosition,
                MessagePosition = messagePosition,
                ImageType = imageType,
                DataUrl = GetDataUrl(imageType, mediaLibrary),
                MediaLibrary = mediaLibrary
            };

            return View("~/Views/Shared/Components/ImageUploaderEditor/_ImageUploaderEditor.cshtml", model);
        }


        private string GetDataUrl(ImageTypeEnum imageType, string mediaLibrary)
        {
            if (imageType == ImageTypeEnum.Attachment)
            {
                var url = Url.Action(new UrlActionContext
                {
                    Action = "Upload",
                    Controller = "AttachmentImageUploader",
                    Values = new
                    {
                        pageId = _pageDataContextRetriever.Retrieve<TreeNode>().Page.DocumentID
                    }
                });

                return Url.Kentico().AuthenticateUrlRaw(url, false);
            }

            if (imageType == ImageTypeEnum.MediaFile)
            {
                var url = Url.Action(new UrlActionContext
                {
                    Action = "Upload",
                    Controller = "MediaFileImageUploader",
                    Values = new
                    {
                        libraryName = mediaLibrary
                    },
                    
                });;

                return Url.Kentico().AuthenticateUrlRaw(url, false);
            }

            return null;
        }
    }
}
