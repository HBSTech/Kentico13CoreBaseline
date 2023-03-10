using CMS.DocumentEngine;
using ImageUploaderInlineControl.Models;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Threading.Tasks;

namespace ImageUploaderInlineControl.Components.InlineEditors.ImageUploaderEditor
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


        public async Task<IViewComponentResult> InvokeAsync(string propertyName, bool hasImage, UploaderImageTypeEnum imageType, bool useAbsolutePosition, 
            PanelPositionEnum messagePosition, string? mediaLibrary, string? mediaLibrarySubFolder)
        {
            string mediaLibrary = await _siteSettingsRepository.GetImageUploadMediaLibraryAsync();
            var model = new ImageUploaderEditorViewModel
            {
                PropertyName = propertyName,
                HasImage = hasImage,
                UseAbsolutePosition = useAbsolutePosition,
                MessagePosition = messagePosition,
                ImageType = imageType,
                DataUrl = GetDataUrl(imageType, mediaLibrary.AsNullOrWhitespaceMaybe(), mediaLibrarySubFolder.AsNullOrWhitespaceMaybe()),
                MediaLibrary = mediaLibrary
            };

            return View("~/Views/Shared/Components/ImageUploaderEditor/_ImageUploaderEditor.cshtml", model);
        }


        private Result<string> GetDataUrl(UploaderImageTypeEnum imageType, Maybe<string> mediaLibrary, Maybe<string> subFolder)
        {
            string resolvedUrl = string.Empty;
            if (imageType == UploaderImageTypeEnum.Attachment)
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

                resolvedUrl = Url.Kentico().AuthenticateUrlRaw(url, false);
            }

            if (imageType == UploaderImageTypeEnum.MediaFile && mediaLibrary.TryGetValue(out var mediaLibraryName))
            {
                var url = Url.Action(new UrlActionContext
                {
                    Action = "Upload",
                    Controller = "MediaFileImageUploader",
                    Values = new
                    {
                        libraryName = mediaLibraryName,
                        subFolder = subFolder.GetValueOrDefault("uploaded")
                    }
                });

                resolvedUrl = Url.Kentico().AuthenticateUrlRaw(url, false);
            }

            return Result.FailureIf(string.IsNullOrWhiteSpace(resolvedUrl), resolvedUrl, "No Url found");
        }
    }
}
