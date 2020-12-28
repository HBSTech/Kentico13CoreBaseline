using Generic.Controllers;
using CMS.DocumentEngine;
using System.IO;
using File = CMS.DocumentEngine.Types.Generic.File;
using Microsoft.AspNetCore.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Content.Web.Mvc;
using CMS.Base;
using System.Threading.Tasks;

[assembly: RegisterPageRoute("CMS.File", typeof(FileController), ActionName = nameof(FileController.GetFileGeneric))]
[assembly: RegisterPageRoute(File.CLASS_NAME, typeof(FileController), ActionName = nameof(FileController.GetFile))]
namespace Generic.Controllers
{
    /// <summary>
    /// This allows you to have Generic.File page types that when you visit the URL it renders the file itself.
    /// </summary>
    public class FileController : Controller
    {
        public FileController(IPageDataContextRetriever pageDataContextRetriever,
            ISiteService siteService,
            IAttachmentInfoProvider attachmentInfoProvider)
        {
            PageDataContextRetriever = pageDataContextRetriever;
            SiteService = siteService;
            AttachmentInfoProvider = attachmentInfoProvider;
        }

        public IPageDataContextRetriever PageDataContextRetriever { get; }
        public ISiteService SiteService { get; }
        public IAttachmentInfoProvider AttachmentInfoProvider { get; }

        public async Task<ActionResult> GetFile()
        {
            var PageContext = PageDataContextRetriever.Retrieve<File>();
            if (PageContext.Page != null)
            {
                var Attachment = await AttachmentInfoProvider.GetAsync(PageContext.Page.FileAttachment, SiteService.CurrentSite.SiteID);
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

        public async Task<ActionResult> GetFileGeneric()
        {
            var PageContext = PageDataContextRetriever.Retrieve<TreeNode>();
            if (PageContext.Page != null && PageContext.Page.Attachments.Count > 0)
            {
                var Attachment = await AttachmentInfoProvider.GetAsync(PageContext.Page.Attachments.FirstItem.AttachmentGUID, SiteService.CurrentSite.SiteID);
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