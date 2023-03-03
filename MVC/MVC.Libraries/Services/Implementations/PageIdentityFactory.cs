using AutoMapper;
using Generic.Models;
using Generic.Services.Interfaces;

namespace Generic.Services.Implementations
{
    [AutoDependencyInjection]
    public class PageIdentityFactory : IPageIdentityFactory
    {
        private readonly IMapper _mapper;

        public PageIdentityFactory(IMapper mapper)
        {
            _mapper = mapper;
        }

        public PageIdentity<TData> Convert<TData, TOriginalData>(PageIdentity<TOriginalData> pageIdentity)
        {
            // Convert data using AutoMapper
            TData data = (pageIdentity.Data != null ? _mapper.Map<TData>(pageIdentity.Data) : default);

            return new PageIdentity<TData>()
            {
                Data = data,
                DocumentGUID = pageIdentity.DocumentGUID,
                DocumentID = pageIdentity.DocumentID,
                NodeGUID = pageIdentity.NodeGUID,
                NodeID = pageIdentity.NodeID,
                NodeLevel = pageIdentity.NodeLevel,
                AbsoluteUrl = pageIdentity.AbsoluteUrl,
                RelativeUrl = pageIdentity.RelativeUrl,
                Alias = pageIdentity.Alias,
                Name = pageIdentity.Name,
                Path = pageIdentity.Path
            };
        }
    }
}
