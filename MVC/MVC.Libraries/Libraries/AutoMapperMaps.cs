using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.Search;
using Generic.Models;
using Kentico.Content.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using static AutoMapper.AutoMapperConfigurationException;
using static CMS.DocumentEngine.Types.Generic.Tab;
using static CMS.DocumentEngine.Types.Generic.TabParent;

namespace Generic.Libraries.AutoMapper
{
    public class AutoMapperMaps : Profile
    {
        public AutoMapperMaps()
        {
            CreateMap<TreeNode, Breadcrumb>()
                .ForMember(dest => dest.LinkText, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.LinkUrl, opt => opt.MapFrom(src => DocumentURLProvider.GetUrl(src).Replace("~", "")))
                .ForMember(dest => dest.IsCurrentPage, opt => opt.Ignore());

            // Used for when we get a NavItem from cache so the List of it is not the same
            CreateMap<NavigationItem, NavigationItem>()
                .ForMember(dest => dest.Children, opt => opt.Ignore());

            // Page to Navigation Item
            CreateMap<TreeNode, NavigationItem>()
                .BeforeMap((s, d) => d.LinkTarget = "_self")
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.LinkText, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.LinkHref, opt => opt.MapFrom(src => DocumentURLProvider.GetUrl(src).Replace("~", "")))
                .ForMember(dest => dest.LinkPagePath, opt => opt.MapFrom(src => src.NodeAliasPath))
                .ForMember(dest => dest.LinkPageGUID, opt => opt.MapFrom(src => src.NodeGUID))
                .ForMember(dest => dest.LinkPageID, opt => opt.MapFrom(src => src.NodeID))
                .ForMember(dest => dest.LinkDocumentID, opt => opt.MapFrom(src => src.DocumentID))
                .ForMember(dest => dest.LinkDocumentGUID, opt => opt.MapFrom(src => src.DocumentGUID));

            CreateMap<DocumentAttachment, AttachmentItem>()
                .AfterMap<DocumentAttachmentUrlMapping>();

            CreateMap<AttachmentInfo, AttachmentItem>()
                .AfterMap<AttachmentUrlMapping>();

            CreateMap<MediaFileInfo, MediaItem>()
                .ForMember(dest => dest.MediaGUID, opt => opt.MapFrom(src => src.FileGUID))
                .ForMember(dest => dest.MediaName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.MediaDescription, opt => opt.MapFrom(src => src.FileDescription))
                .ForMember(dest => dest.MediaExtension, opt => opt.MapFrom(src => src.FileExtension))
                .AfterMap<MediaUrlMapping>();

            CreateMap<UserInfo, User>();


            CreateMap<RoleInfo, RoleItem>();

            CreateMap<TreeNode, SitemapNode>()
                .ForMember(dest => dest.Url, opt => opt.Ignore())
                .ForMember(dest => dest.LastModificationDate, opt => opt.MapFrom(src => src.DocumentModifiedWhen))
                .ForMember(dest => dest.ChangeFrequency, opt => opt.Ignore())
                .AfterMap<SiteMapNodeMapping>();

            CreateMap<Tab, TabItem>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TabName));

            CreateMap<TabParent, TabParentItem>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TabParentName))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.NodeAliasPath));

            // Use IMappAction below to add new fields based on the types and stuff
            CreateMap<SearchResultItem, SearchItem>()
                .AfterMap<SearchItemMapping>();

            VerifyMaps();
        }

        private void VerifyMaps()
        {
            var invalidConfigurations = new List<TypeMapConfigErrors>();
            ForAllMaps((cfg, test) =>
            {
                if (!(cfg.IsValid ?? true))
                {
                    invalidConfigurations.Add(new TypeMapConfigErrors(cfg, cfg.GetUnmappedPropertyNames(), false));
                }
            });
            if (invalidConfigurations.Any())
            {
                throw new AutoMapperConfigurationException(invalidConfigurations.ToArray());
            }
        }
    }

    public class AttachmentUrlMapping : IMappingAction<AttachmentInfo, AttachmentItem>
    {
        private readonly IPageAttachmentUrlRetriever _pageAttachmentUrlRetriever;

        public AttachmentUrlMapping(IPageAttachmentUrlRetriever pageAttachmentUrlRetriever)
        {
            _pageAttachmentUrlRetriever = pageAttachmentUrlRetriever;
        }

        public void Process(AttachmentInfo source, AttachmentItem destination, ResolutionContext context)
        {
            destination.AttachmentUrl = _pageAttachmentUrlRetriever.Retrieve(source).RelativePath;
            destination.AttachmentTitle = !string.IsNullOrWhiteSpace(source.AttachmentTitle) ? source.AttachmentTitle : source.AttachmentName;
            destination.AttachmentExtension = CMS.IO.Path.GetExtension(source.AttachmentName);
        }
    }
    public class DocumentAttachmentUrlMapping : IMappingAction<DocumentAttachment, AttachmentItem>
    {
        private readonly IPageAttachmentUrlRetriever _pageAttachmentUrlRetriever;

        public DocumentAttachmentUrlMapping(IPageAttachmentUrlRetriever pageAttachmentUrlRetriever)
        {
            _pageAttachmentUrlRetriever = pageAttachmentUrlRetriever;
        }

        public void Process(DocumentAttachment source, AttachmentItem destination, ResolutionContext context)
        {
            destination.AttachmentUrl = _pageAttachmentUrlRetriever.Retrieve(source).RelativePath;
            destination.AttachmentTitle = !string.IsNullOrWhiteSpace(source.AttachmentTitle) ? source.AttachmentTitle : source.AttachmentName;
            destination.AttachmentExtension = CMS.IO.Path.GetExtension(source.AttachmentName);
        }
    }

    public class MediaUrlMapping : IMappingAction<MediaFileInfo, MediaItem>
    {
        private readonly IMediaFileUrlRetriever _mediaFileUrlRetriever;

        public MediaUrlMapping(IMediaFileUrlRetriever mediaFileUrlRetriever)
        {
            _mediaFileUrlRetriever = mediaFileUrlRetriever;
        }

        public void Process(MediaFileInfo source, MediaItem destination, ResolutionContext context)
        {
            var mediaFileUrl = _mediaFileUrlRetriever.Retrieve(source);
            destination.MediaPermanentUrl = mediaFileUrl.RelativePath;
            destination.MediaUrl = mediaFileUrl.DirectPath;
            destination.MediaTitle = !string.IsNullOrWhiteSpace(source.FileTitle) ? source.FileTitle : source.FileName;
        }
    }
    public class SiteMapNodeMapping : IMappingAction<TreeNode, SitemapNode>
    {
        private readonly IPageUrlRetriever _pageUrlRetriever;

        public SiteMapNodeMapping(IPageUrlRetriever pageUrlRetriever)
        {
            _pageUrlRetriever = pageUrlRetriever;
        }

        public void Process(TreeNode source, SitemapNode destination, ResolutionContext context)
        {
            destination.Url = _pageUrlRetriever.Retrieve(source).AbsoluteUrl;
        }
    }


    public class SearchItemMapping : IMappingAction<SearchResultItem, SearchItem>
    {
        private readonly IPageUrlRetriever _pageUrlRetriever;

        public SearchItemMapping(IPageUrlRetriever pageUrlRetriever)
        {
            _pageUrlRetriever = pageUrlRetriever;
        }

        public void Process(SearchResultItem source, SearchItem destination, ResolutionContext context)
        {
            if(source.Data is TreeNode page)
            {
                destination.IsPage = true;
                destination.PageUrl = _pageUrlRetriever.Retrieve(page).RelativePath;
            }
            // Example of adjusting search
            // if(source.Type.Equals(Tab.CLASS_NAME, System.StringComparison.InvariantCultureIgnoreCase)) {
            //  destination.TabName = source.GetSearchValue("TabName");
            // }
        }
    }
}