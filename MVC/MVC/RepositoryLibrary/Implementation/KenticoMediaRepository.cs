using CMS.Base;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.MediaLibrary;
using Generic.Repositories.Interfaces;
using Kentico.Content.Web.Mvc;
using MVCCaching;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    public class KenticoMediaRepository : IMediaRepository
    {
        private ISiteService _SiteRepo;
        private IPageAttachmentUrlRetriever _pageAttachmentUrlRetriever;
        private IAttachmentInfoProvider _attachmentInfoProvider;
        private IMediaFileInfoProvider _mediaFileInfoProvider;
        private IEventLogService _eventLogService;

        public KenticoMediaRepository(ISiteService SiteRepo, 
            IPageAttachmentUrlRetriever pageAttachmentUrlRetriever, 
            IAttachmentInfoProvider attachmentInfoProvider,
            IMediaFileInfoProvider mediaFileInfoProvider,
            IEventLogService eventLogService)
        {
            _SiteRepo = SiteRepo;
            _pageAttachmentUrlRetriever = pageAttachmentUrlRetriever;
            _attachmentInfoProvider = attachmentInfoProvider;
            _mediaFileInfoProvider = mediaFileInfoProvider;
            _eventLogService = eventLogService;
        }

        [CacheDependency("attachment|{1}")]
        public string GetAttachmentImage(TreeNode Page, Guid ImageGuid)
        {
            return GetAttachmentImageAsync(Page, ImageGuid).Result;
        }

            [CacheDependency("attachment|{1}")]
        public async Task<string> GetAttachmentImageAsync(TreeNode Page, Guid ImageGuid)
        {
            var Attachment = Page?.AllAttachments.Where(x => x.AttachmentGUID == ImageGuid).FirstOrDefault();
            return (Attachment != null ? _pageAttachmentUrlRetriever.Retrieve(Attachment).RelativePath : "");
        }

        [CacheDependency("attachment|{0}")]
        public string GetAttachmentImage(Guid ImageGuid)
        {
            return GetAttachmentImageAsync(ImageGuid).Result;
        }

            [CacheDependency("attachment|{0}")]
        public async Task<string> GetAttachmentImageAsync(Guid ImageGuid)
        {
            var Attachment = _attachmentInfoProvider.GetWithoutBinary(ImageGuid, _SiteRepo.CurrentSite.SiteID);
            return (Attachment != null ? _pageAttachmentUrlRetriever.Retrieve(Attachment).RelativePath : "");
        }

        [CacheDependency("mediafile|{0}")]
        public string GetMediaFileUrl(Guid FileGuid)
        {
            return GetMediaFileUrlAsync(FileGuid).Result;
        }

            [CacheDependency("mediafile|{0}")]
        public async Task<string> GetMediaFileUrlAsync(Guid FileGuid)
        {
            try
            {
                return MediaLibraryHelper.GetDirectUrl(_mediaFileInfoProvider.Get(FileGuid, _SiteRepo.CurrentSite.SiteID));
            }
            catch (Exception ex)
            {
                _eventLogService.LogException("KenticoMediaRepository", "MediaFileMissing", ex, additionalMessage: "For media file with Guid " + FileGuid.ToString());
                return "";
            }
        }
    }
}