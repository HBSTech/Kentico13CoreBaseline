using AutoMapper;
using CMS.DocumentEngine;
using CMS.Membership;
using Generic.Models;
using Generic.Models.User;

namespace Generic.AutoMapper
{
    public class AutoMapperMaps : Profile
    {
        public AutoMapperMaps()
        {
            CreateMap<TreeNode, Breadcrumb>()
                .ForMember(dest => dest.LinkText, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.LinkUrl, opt => opt.MapFrom(src => DocumentURLProvider.GetUrl(src)));

            // Used for when we get a NavItem from cache so the List of it is not the same
            CreateMap<NavigationItem, NavigationItem>()
                .ForMember(dest => dest.Children, opt => opt.Ignore());

            // Page to Navigation Item
            CreateMap<TreeNode, NavigationItem>()
                .BeforeMap((s, d) => d.LinkTarget = "_self")
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.LinkText, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.LinkHref, opt => opt.MapFrom(src => DocumentURLProvider.GetUrl(src)))
                .ForMember(dest => dest.LinkPagePath, opt => opt.MapFrom(src => src.NodeAliasPath))
                .ForMember(dest => dest.LinkPageGuid, opt => opt.MapFrom(src => src.NodeGUID))
                .ForMember(dest => dest.LinkPageID, opt => opt.MapFrom(src => src.NodeID))
                .ForMember(dest => dest.LinkDocumentID, opt => opt.MapFrom(src => src.DocumentID))
                .ForMember(dest => dest.LinkDocumentGuid, opt => opt.MapFrom(src => src.DocumentGUID));

            CreateMap<BasicUser, UserInfo>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail));
        }
    }
}