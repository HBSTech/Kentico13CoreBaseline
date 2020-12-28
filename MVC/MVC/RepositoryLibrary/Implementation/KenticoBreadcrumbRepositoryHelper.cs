using AutoMapper;
using CMS.DocumentEngine;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoBreadcrumbRepositoryHelper : IKenticoBreadcrumbRepositoryHelper
    {
        private IRepoContext _repoContext;
        private IMapper _Mapper;

        public KenticoBreadcrumbRepositoryHelper(IRepoContext repoContext, IMapper Mapper)
        {
            _repoContext = repoContext;
            _Mapper = Mapper;
        }

        [CacheDependency("Nodeid|{0}")]
        public TreeNode GetBreadcrumbNode(int NodeID)
        {
            return GetBreadcrumbNodeAsync(NodeID).Result;
        }

        [CacheDependency("Nodeid|{0}")]
        public async Task<TreeNode> GetBreadcrumbNodeAsync(int NodeID)
        {
            return DocumentHelper.GetDocuments()
                .WhereEquals("NodeID", NodeID)
                .Culture(_repoContext.CurrentCulture())
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!_repoContext.PreviewEnabled())
                .LatestVersion(_repoContext.PreviewEnabled())
                .FirstOrDefault();
        }

        public Breadcrumb PageToBreadcrumb(TreeNode Page, bool IsCurrentPage)
        {
            return PageToBreadcrumbAsync(Page, IsCurrentPage).Result;
        }

        /// <summary>
        /// Converts the TreeNode into a breadcrumb.
        /// </summary>
        /// <param name="Page">The Page</param>
        /// <param name="IsCurrentPage">If the page is the current page</param>
        /// <returns></returns>
        public async Task<Breadcrumb> PageToBreadcrumbAsync(TreeNode Page, bool IsCurrentPage)
        {
            Breadcrumb breadcrumb = _Mapper.Map<Breadcrumb>(Page);
            breadcrumb.IsCurrentPage = IsCurrentPage;
            return breadcrumb;
        }
    }
}