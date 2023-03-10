using CMS.DocumentEngine;
using File = CMS.DocumentEngine.Types.Generic.File;
using Microsoft.AspNetCore.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Content.Web.Mvc;
using CMS.Base;
using FileContentType.RCL.KX13.Features.File;

[assembly: RegisterPageRoute("CMS.File", typeof(FileController), ActionName = nameof(FileController.GetFileGeneral))]
[assembly: RegisterPageRoute(File.CLASS_NAME, typeof(FileController), ActionName = nameof(FileController.GetFile))]
namespace FileContentType.RCL.KX13.Features.File
{
    /// <summary>
    /// This allows you to have Generic.File page types that when you visit the URL it renders the file itself.
    /// Leaving this 'kentico integrated' as depends on page builder anyway to hit this.
    /// </summary>
    public class FileController : Controller
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;
        private readonly ISiteService _siteService;
        private readonly IAttachmentInfoProvider _attachmentInfoProvider;

        public FileController(IPageDataContextRetriever pageDataContextRetriever,
            ISiteService siteService,
            IAttachmentInfoProvider attachmentInfoProvider)
        {
            _pageDataContextRetriever = pageDataContextRetriever;
            _siteService = siteService;
            _attachmentInfoProvider = attachmentInfoProvider;
        }


        public async Task<ActionResult> GetFile()
        {
            var PageContext = _pageDataContextRetriever.Retrieve<CMS.DocumentEngine.Types.Generic.File>();
            if (PageContext.Page != null)
            {
                var Attachment = await _attachmentInfoProvider.GetAsync(PageContext.Page.FileAttachment, _siteService.CurrentSite.SiteID);
                if (Attachment != null)
                {
                    return new FileStreamResult(new MemoryStream(Attachment.AttachmentBinary), Attachment.AttachmentMimeType)
                    {
                        FileDownloadName = Attachment.AttachmentName
                    };
                }
            }

            return NotFound();
        }

        public async Task<ActionResult> GetFileGeneral()
        {
            var PageContext = _pageDataContextRetriever.Retrieve<TreeNode>();
            if (PageContext.Page != null && PageContext.Page.Attachments.Any())
            {
                var Attachment = await _attachmentInfoProvider.GetAsync(PageContext.Page.Attachments.FirstItem.AttachmentGUID, _siteService.CurrentSite.SiteID);
                if (Attachment != null)
                {
                    return new FileStreamResult(new MemoryStream(Attachment.AttachmentBinary), Attachment.AttachmentMimeType)
                    {
                        FileDownloadName = Attachment.AttachmentName
                    };
                }
            }

            return NotFound();
        }
    }
}