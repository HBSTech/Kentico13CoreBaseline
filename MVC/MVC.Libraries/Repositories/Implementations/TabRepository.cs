using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
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
    public class TabRepository : ITabRepository
    {
        private readonly IPageDataContextRetriever _pageDataContextRetriever;
        private readonly IPageRetriever _pageRetriever;
        private readonly IMapper _mapper;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteRepository _siteRepository;

        public TabRepository(IPageDataContextRetriever pageDataContextRetriever,
            IPageRetriever pageRetriever,
            IMapper mapper,
            ICacheDependenciesStore cacheDependenciesStore,
            ISiteRepository siteRepository)
        {
            _pageDataContextRetriever = pageDataContextRetriever;
            _pageRetriever = pageRetriever;
            _mapper = mapper;
            _cacheDependenciesStore = cacheDependenciesStore;
            _siteRepository = siteRepository;
        }

        public async Task<TabParentItem> GetTabParentAsync(int nodeID)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore);
            builder.Node(nodeID);

            // See if current page is the page, this avoids un-needed database queries
            if(_pageDataContextRetriever.TryRetrieve<TabParent>(out var page))
            {
                if (page.Page.NodeID.Equals(nodeID)) {
                    return _mapper.Map<TabParentItem>(page.Page);
                }
            }

            // Do normal lookup
            var retriever = await _pageRetriever.RetrieveAsync<TabParent>(
               query => query
                   .WhereEquals(nameof(TreeNode.NodeID), nodeID)
                   .Columns(new string[] {
                        nameof(TabParent.TabParentName),
                        nameof(TabParent.NodeAliasPath)
                   }),
               cs => cs.Configure(builder, 60, "GetTabParentAsync", nodeID)
               );

            return retriever.Select(x => _mapper.Map<TabParentItem>(x)).FirstOrDefault();
        }

        public async Task<IEnumerable<TabItem>> GetTabsAsync(string path)
        {
            var builder = new CacheDependencyKeysBuilder(_siteRepository, _cacheDependenciesStore)
                .PagePath(path, PathTypeEnum.Children);

            var retriever = await _pageRetriever.RetrieveAsync<Tab>(
                query => query
                    .Path(path, PathTypeEnum.Children)
                    .Columns(new string[] {
                        nameof(Tab.DocumentID),
                        nameof(Tab.TabName)
                    })
                    .OrderBy(nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeOrder)),
                cs => cs.Configure(builder, 60, "GetTabsAsync", path)
                );

            return retriever.Select(x => _mapper.Map<TabItem>(x));
        }
    }
}
