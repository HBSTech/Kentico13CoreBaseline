using CMS.Base.UploadExtensions;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.PageBuilder.Web.Mvc;

namespace ImageUploaderInlineControl.Components.InlineEditors.ImageUploaderEditor
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaFileImageUploaderController : ControllerBase
    {
        private readonly IPageBuilderDataContextRetriever _dataContextRetriever;


        public MediaFileImageUploaderController(IPageBuilderDataContextRetriever dataContextRetriever)
        {
            _dataContextRetriever = dataContextRetriever;
        }


        [HttpPost]
        public object Upload(string libraryName, string subFolder = "uploaded")
        {
            var dataContext = _dataContextRetriever.Retrieve();
            if (!dataContext.EditMode)
            {
                return StatusCode(403, new ObjectResult("It is allowed to upload an image only when the page builder is in the edit mode."));
            }

            var library = MediaLibraryInfo.Provider.Get(libraryName, SiteContext.CurrentSiteID);
            if (library == null)
            {
                return NotFound($"The '{libraryName}' media library doesn't exist.");
            }

            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(library, "FileCreate", MembershipContext.AuthenticatedUser))
            {
                return StatusCode(403, new ObjectResult("You are not authorized to upload an image to the media library."));
            }

            var imageGuid = Guid.Empty;

            foreach (var requestFile in Request.Form.Files)
            {
                var failedValidationResult = ImageUploaderHelper.ValidateUploadedFile(requestFile);

                if (failedValidationResult != null)
                {
                    return failedValidationResult;
                }

                imageGuid = AddMediaFile(requestFile, library, subFolder);
            }

            return new { guid = imageGuid };
        }


        private static Guid AddMediaFile(IFormFile requestFile, MediaLibraryInfo library, string subFolder)
        {
            var mediaFile = new MediaFileInfo(requestFile.ToUploadedFile(), library.LibraryID, subFolder);
            MediaFileInfo.Provider.Set(mediaFile);
            return mediaFile.FileGUID;
        }
    }
}