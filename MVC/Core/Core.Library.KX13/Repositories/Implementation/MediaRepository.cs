
using CMS.DataEngine;
using CMS.MediaLibrary;
using System.Data;

namespace Core.Repositories.Implementation
{
    [AutoDependencyInjection]
    public class MediaRepository : IMediaRepository
    {

        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ISiteRepository _siteRepository;
        private readonly IPageRetriever _pageRetriever;
        private readonly IProgressiveCache _progressiveCache;
        private readonly IAttachmentInfoProvider _attachmentInfoProvider;
        private readonly IMediaFileInfoProvider _mediaFileInfoProvider;
        private readonly IPageDataContextRetriever _pageDataContextRetriever;
        private readonly IMediaFileUrlRetriever _mediaFileUrlRetriever;
        private readonly IPageAttachmentUrlRetriever _pageAttachmentUrlRetriever;

        public MediaRepository(ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ISiteRepository siteRepository,
            IPageRetriever pageRetriever,
            IProgressiveCache progressiveCache,
            IAttachmentInfoProvider attachmentInfoProvider,
            IMediaFileInfoProvider mediaFileInfoProvider,
            IPageDataContextRetriever pageDataContextRetriever,
            IMediaFileUrlRetriever mediaFileUrlRetriever,
            IPageAttachmentUrlRetriever pageAttachmentUrlRetriever)
        {
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _siteRepository = siteRepository;
            _pageRetriever = pageRetriever;
            _progressiveCache = progressiveCache;
            _attachmentInfoProvider = attachmentInfoProvider;
            _mediaFileInfoProvider = mediaFileInfoProvider;
            _pageDataContextRetriever = pageDataContextRetriever;
            _mediaFileUrlRetriever = mediaFileUrlRetriever;
            _pageAttachmentUrlRetriever = pageAttachmentUrlRetriever;
        }

        public async Task<Result<MediaItem>> GetAttachmentItemAsync(Guid attachmentGuid)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Attachment(attachmentGuid);
            var result = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                var attachment = await _attachmentInfoProvider.Get()
                .WhereEquals(nameof(AttachmentInfo.AttachmentGUID), attachmentGuid)
                .TopN(1)
                .BinaryData(false)
                .GetEnumerableTypedResultAsync();
                return attachment.FirstOrMaybe();

            }, new CacheSettings(15, "GetAttachmentItemAsnc", attachmentGuid));

            return result.HasValue ? AttachmentToMediaItem(result.Value) : Result.Failure<MediaItem>("Could not find attachment");
        }

        public async Task<Result<MediaItem>> GetMediaItemAsync(Guid fileGuid)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Object(MediaFileInfo.OBJECT_TYPE, fileGuid);
            var result = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _mediaFileInfoProvider.GetAsync(fileGuid, await _siteRepository.GetSiteIDAsync());
            }, new CacheSettings(15, "GetMediaItemAsync", fileGuid));
            if (result != null)
            {
                return Result.Success(MediaFileInfoToMediaItem(result));
            }
            return Result.Failure<MediaItem>("Could not find Media Item");
        }

        public async Task<Result<MediaItem>> GetPageAttachmentAsync(Guid attachmentGuid, int? documentID = null)
        {
            var builder = _cacheDependencyBuilderFactory.Create();

            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage) && (!documentID.HasValue || currentPage.Page.DocumentID == documentID.Value))
            {
                var attachment = currentPage.Page.Attachments?.Where(x => x.AttachmentGUID.Equals(attachmentGuid)).FirstOrDefault();
                if (attachment != null)
                {
                    builder.Attachment(attachmentGuid);
                    return AttachmentToMediaItem(attachment);
                }
            }

            // Look up normally
            return await GetAttachmentItemAsync(attachmentGuid);
        }

        public async Task<IEnumerable<MediaItem>> GetPageAttachmentsAsync(int? documentID = null)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            if (_pageDataContextRetriever.TryRetrieve<TreeNode>(out var currentPage) && (!documentID.HasValue || currentPage.Page.DocumentID == documentID.Value))
            {
                currentPage.Page.Attachments.ToList().ForEach(x => builder.Attachment(x.AttachmentGUID));
                builder.Page(currentPage.Page.DocumentID);
                return currentPage.Page.Attachments.ToList().Select(x => AttachmentToMediaItem(x));
            }

            // Do lookup for page
            int docId = documentID.GetValueOrDefault(0);
            builder.Page(docId);
            var results = await _pageRetriever.RetrieveAsync<TreeNode>(
                query => query
                    .WhereEquals(nameof(TreeNode.DocumentID), docId),
                cacheSettings => cacheSettings
                    .Dependencies((items, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
                    .Key($"GetPageAttachmentsAsync|{docId}")
                    .Expiration(TimeSpan.FromMinutes(15))
                );

            if (results.Any())
            {
                return results.First().Attachments.ToList().Select(x => AttachmentToMediaItem(x));
            }
            else
            {
                return Array.Empty<MediaItem>();
            }
        }

        public async Task<IEnumerable<MediaItem>> GetMediaItemsByLibraryAsync(string libraryName)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            builder.Object(MediaLibraryInfo.OBJECT_TYPE, libraryName);

            var result = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }

                // Gets an instance of the 'SampleMediaLibrary' media library for the current site
                return await _mediaFileInfoProvider.Get()
                        .Source(x => x.Join<MediaLibraryInfo>(nameof(MediaFileInfo.FileLibraryID), nameof(MediaLibraryInfo.LibraryID)))
                        .WhereEquals(nameof(MediaLibraryInfo.LibraryName), libraryName)
                        .GetEnumerableTypedResultAsync();
            }, new CacheSettings(15, "GetMediaItemsByLibraryAsync", libraryName));

            return result.Select(x => MediaFileInfoToMediaItem(x));
        }

        public async Task<IEnumerable<MediaItem>> GetMediaItemsByPathAsync(string path, string? libraryName = null)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            if (libraryName.AsMaybe().TryGetValue(out var libraryNameVal))
            {
                builder.Object(MediaLibraryInfo.OBJECT_TYPE, libraryNameVal);
            }

            var result = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }

                // Gets an instance of the 'SampleMediaLibrary' media library for the current site
                return await _mediaFileInfoProvider.Get()
                        .Source(x => x.Join<MediaLibraryInfo>(nameof(MediaFileInfo.FileLibraryID), nameof(MediaLibraryInfo.LibraryID)))
                        .If(libraryName.AsMaybe().TryGetValue(out var libraryNameVal), query => query.WhereEquals(nameof(MediaLibraryInfo.LibraryName), libraryNameVal))
                        .WhereLike(nameof(MediaFileInfo.FilePath), $"%{SqlHelper.EscapeLikeText(path)}%")
                        .GetEnumerableTypedResultAsync();
            }, new CacheSettings(15, "GetMediaItemsByPathAsync", path, libraryName));

            return result.Select(x => MediaFileInfoToMediaItem(x));
        }

        public async Task<IEnumerable<MediaItem>> GetAttachmentsAsync(IEnumerable<Guid> attachmentGuids)
        {
            var builder = _cacheDependencyBuilderFactory.Create();
            foreach (var attachment in attachmentGuids)
            {
                builder.Attachment(attachment);
            }

            var attachmentInfos = await _progressiveCache.LoadAsync(async cs =>
            {
                cs.CacheDependency = builder.GetCMSCacheDependency();
                var attachments = await _attachmentInfoProvider.Get().WhereIn(nameof(AttachmentInfo.AttachmentGUID), attachmentGuids.ToArray()).GetEnumerableTypedResultAsync();
                return attachments;
            }, new CacheSettings(Convert.ToDouble((int)CacheMinuteTypes.Medium), "GetAttachments", attachmentGuids.Select(x => x.ToString()).Join(",")));

            return attachmentInfos.Select(x => AttachmentToMediaItem(x));
        }

        public async Task<Result<string>> GetMediaAttachmentSiteNameAsync(Guid mediaOrAttachmentGuid)
        {
            // Get dictionary of Guid to SiteID
            var guidToSiteID = await _progressiveCache.LoadAsync(async cs =>
            {
                var data = await XperienceCommunityConnectionHelper.ExecuteQueryAsync(
                    @"select FileGuid as [Guid], FileSiteID as [SiteId] from Media_File
                        union all
                    Select AttachmentGUID as [Guid], AttachmentSiteID as [SiteId] from CMS_Attachment", new QueryDataParameters(), QueryTypeEnum.SQLQuery);

                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(new string[]
                    {
                        $"{MediaFileInfo.OBJECT_TYPE}|all",
                        $"{AttachmentInfo.OBJECT_TYPE}|all",
                    });
                }

                return data.Tables[0].Rows.Cast<DataRow>()
                    .Select(x => new Tuple<Guid, int>(ValidationHelper.GetGuid(x["Guid"], Guid.Empty), ValidationHelper.GetInteger(x["SiteID"], 0)))
                    .GroupBy(x => x.Item1)
                    .ToDictionary(key => key.Key, value => value.First().Item2);

            }, new CacheSettings(CacheMinuteTypes.Long.ToDouble(), "GetAttachmentMediaGuidToSiteID"));

            if (guidToSiteID.GetValueOrMaybe(mediaOrAttachmentGuid).TryGetValue(out var siteID))
            {
                string siteName = _siteRepository.SiteNameById(siteID);
                return siteName;
            }
            return Result.Failure<string>("Could not find attachment or media item by that Guid");

        }

        private MediaItem MediaFileInfoToMediaItem(MediaFileInfo mediaFile)
        {
            var fileUrl = _mediaFileUrlRetriever.Retrieve(mediaFile);
            return new MediaItem(
                mediaGUID: mediaFile.FileGUID,
                mediaName: mediaFile.FileName,
                mediaTitle: mediaFile.FileTitle.AsNullOrWhitespaceMaybe().GetValueOrDefault(mediaFile.FileName),
                mediaExtension: mediaFile.FileExtension,
                mediaUrl: fileUrl.DirectPath,
                mediaPermanentUrl: fileUrl.RelativePath
                )
            {
                MediaDescription = mediaFile.FileDescription.AsNullOrWhitespaceMaybe()
            };

        }

        private MediaItem AttachmentToMediaItem(AttachmentInfo attachment)
        {
            var urls = _pageAttachmentUrlRetriever.Retrieve(attachment);
            var attachmentItem = new MediaItem(
                mediaName: attachment.AttachmentName,
                mediaGUID: attachment.AttachmentGUID,
                mediaUrl: urls.RelativePath,
                mediaPermanentUrl: urls.RelativePath,
                mediaTitle: attachment.AttachmentTitle.AsNullOrWhitespaceMaybe().GetValueOrDefault(attachment.AttachmentName),
                mediaExtension: attachment.AttachmentExtension
            );
            return attachmentItem;
        }

        private MediaItem AttachmentToMediaItem(DocumentAttachment attachment)
        {
            var urls = _pageAttachmentUrlRetriever.Retrieve(attachment);

            var attachmentItem = new MediaItem(
                mediaName: attachment.AttachmentName,
                mediaGUID: attachment.AttachmentGUID,
                mediaUrl: urls.RelativePath,
                mediaPermanentUrl: urls.RelativePath,
                mediaTitle: attachment.AttachmentTitle.AsNullOrWhitespaceMaybe().GetValueOrDefault(attachment.AttachmentName),
                mediaExtension: attachment.AttachmentExtension
            );
            return attachmentItem;
        }

    }
}