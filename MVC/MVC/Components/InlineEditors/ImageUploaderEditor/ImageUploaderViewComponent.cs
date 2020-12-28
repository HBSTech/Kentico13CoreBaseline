using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Generic.InlineEditors
{
    [ViewComponent(Name = "ImageUploader")]
    public class ImageUploaderViewComponent : ViewComponent
    {
        private const string MEDIA_LIBRARY_NAME = "Graphics";

        private readonly IPageDataContextRetriever pageDataContextRetriever;


        public ImageUploaderViewComponent(IPageDataContextRetriever pageDataContextRetriever)
        {
            this.pageDataContextRetriever = pageDataContextRetriever;
        }


        public IViewComponentResult Invoke(string propertyName, bool hasImage, ImageTypeEnum imageType, bool useAbsolutePosition, 
            PanelPositionEnum messagePosition)
        {
            var model = new ImageUploaderEditorViewModel
            {
                PropertyName = propertyName,
                HasImage = hasImage,
                UseAbsolutePosition = useAbsolutePosition,
                MessagePosition = messagePosition,
                ImageType = imageType,
                DataUrl = GetDataUrl(imageType)
            };

            return View("~/Views/Shared/Components/ImageUploaderEditor/_ImageUploaderEditor.cshtml", model);
        }


        private string GetDataUrl(ImageTypeEnum imageType)
        {
            if (imageType == ImageTypeEnum.Attachment)
            {
                var url = Url.Action(new UrlActionContext
                {
                    Action = "Upload",
                    Controller = "AttachmentImageUploader",
                    Values = new
                    {
                        pageId = pageDataContextRetriever.Retrieve<TreeNode>().Page.DocumentID
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
                        libraryName = MEDIA_LIBRARY_NAME
                    },
                    
                });

                return Url.Kentico().AuthenticateUrlRaw(url, false);
            }

            return null;
        }
    }
}
