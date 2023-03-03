using AutoMapper;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.MediaLibrary;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.Libraries.Helpers;
using Kentico.Content.Web.Mvc;
using MVCCaching.Base.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    [AutoDependencyInjection]
    public class MediaRepository : IMediaRepository
    {
        
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteRepository _siteRepository;
        private readonly IMapper _mapper;
        private readonly IPageRetriever _pageRetriever;
        private readonly IProgressiveCache _progressiveCache;
        private readonly IAttachmentInfoProvider _attachmentInfoProvider;
        private readonly IMediaFileInfoProvider _mediaFileInfoProvider;
        private readonly IPageDataContextRetriever _pageDataContextRetriever;

        public MediaRepository(ICacheDependenciesStore cacheDependenciesStore,
            ISiteRepository siteRepository,
            IMapper mapper,
            IPageRetriever pageRetriever,
            IProgressiveCache progressiveCache,
            IAttachmentInfoProvider attachmentInfoProvider,
            IMediaFileInfoProvider mediaFileInfoProvider,
            IPageDataContextRetriever pageDataContextRetriever)
        {
            _cacheDependenciesStore = cacheDependenciesStore;
            _siteRepository = siteRepository;
            _mapper = mapper;
            _pageRetriever = pageRetriever;
            _progressiveCache = progressiveCache;
            _attachmentInfoProvider = attachmentInfoProvider;
            _mediaFileInfoProvider = mediaFileInfoProvider;
            _pageDataContextRetriever = pageDataContextRetriever;
        }

        public async Task<AttachmentItem> GetAttachmentItemAsync(Guid attachmentGuid)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Attachment(attachmentGuid);
            var result = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _attachmentInfoProvider.GetAsync(attachmentGuid, await _siteRepository.GetSiteIDAsync());
            }, new CacheSettings(15, "GetAttachmentItemAsnc", attachmentGuid));

            return result != null ? _mapper.Map<AttachmentItem>(result) : null;
        }

        public async Task<MediaItem> GetMediaItemAsync(Guid fileGuid)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Object(MediaFileInfo.OBJECT_TYPE, fileGuid);
            var result = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _mediaFileInfoProvider.GetAsync(fileGuid, await _siteRepository.GetSiteIDAsync());
            }, new CacheSettings(15, "GetMediaItemAsync", fileGuid));

            return result != null ? _mapper.Map<MediaItem>(result) : null;
        }

        public async Task<AttachmentItem> GetPageAttachmentAsync(Guid attachmentGuid, int documentID = 0)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);

            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage) && (currentPage.Page.DocumentID == documentID || documentID == 0))
            {
                var attachment = currentPage.Page.Attachments?.Where(x => x.AttachmentGUID.Equals(attachmentGuid)).FirstOrDefault();
                if (attachment != null)
                {
                    builder.Attachment(attachmentGuid);
                    return _mapper.Map<AttachmentItem>(attachment);
                }
            }

            // Look up normally
            return await GetAttachmentItemAsync(attachmentGuid);
        }

        public async Task<IEnumerable<AttachmentItem>> GetPageAttachmentsAsync(int documentID = 0)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage) && (currentPage.Page.DocumentID == documentID || documentID == 0))
            {
                currentPage.Page.Attachments.ToList().ForEach(x => builder.Attachment(x.AttachmentGUID));
                builder.Page(currentPage.Page.DocumentID);
                return currentPage.Page.Attachments.ToList().Select(x => _mapper.Map<AttachmentItem>(x));
            }

            // Do lookup for page
            builder.Page(documentID);
            var results = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentID), documentID),
                cs => cs.Configure(builder, 15, "GetPageAttachmentsAsync", documentID)
                );

            if (results.Any())
            {
                return results.FirstOrDefault().Attachments.ToList().Select(x => _mapper.Map<AttachmentItem>(x));
            } else
            {
                return Array.Empty<AttachmentItem>();
            }
        }
    }
}