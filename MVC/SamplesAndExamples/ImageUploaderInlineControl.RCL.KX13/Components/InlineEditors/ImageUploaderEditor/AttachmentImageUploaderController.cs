using CMS.Base.UploadExtensions;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.PageBuilder.Web.Mvc;

namespace ImageUploaderInlineControl.Components.InlineEditors.ImageUploaderEditor
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentImageUploaderController : ControllerBase
    {
        private readonly IPageBuilderDataContextRetriever dataContextRetriever;


        public AttachmentImageUploaderController(IPageBuilderDataContextRetriever dataContextRetriever)
        {
            this.dataContextRetriever = dataContextRetriever;
        }


        [HttpPost]
        public object Upload(int pageId)
        {
            var dataContext = dataContextRetriever.Retrieve();
            if (!dataContext.EditMode)
            {
                return StatusCode(403, new ObjectResult( "It is allowed to upload an image only when the page builder is in the edit mode."));
            }

            var page = DocumentHelper.GetDocument(pageId, null);
            if (!CheckPagePermissions(page))
            {
                return StatusCode(403, new ObjectResult( "You are not authorized to upload an image to the page."));
            }

            var imageGuid = Guid.Empty;

            foreach (var requestFile in Request.Form.Files)
            {
                imageGuid = AddUnsortedAttachment(page, requestFile);
            }

            return new { guid = imageGuid };
        }


        private static Guid AddUnsortedAttachment(TreeNode page, IFormFile requestFile)
        {
            return DocumentHelper.AddUnsortedAttachment(page, Guid.Empty, requestFile.ToUploadedFile()).AttachmentGUID;
        }


        private static bool CheckPagePermissions(TreeNode page)
        {
            return page?.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser) ?? false;
        }
    }
}