using CMS.DocumentEngine;
using Generic.Models;
using MVCCaching;
using System.Threading.Tasks;

namespace Generic.Repositories.Helpers.Interfaces
{
    public interface IKenticoBreadcrumbRepositoryHelper : IRepository
    {
        /// <summary>
        /// Gets the TreeNode for the given Breadcrumb
        /// </summary>
        /// <param name="NodeID">The NodeID</param>
        /// <returns></returns>
        TreeNode GetBreadcrumbNode(int NodeID);
        Task<TreeNode> GetBreadcrumbNodeAsync(int NodeID);

        /// <summary>
        /// Converts the TreeNode to the breadcrumb
        /// </summary>
        /// <param name="Page">The Page</param>
        /// <param name="IsCurrentPage">If the page is the current page</param>
        /// <returns>The Breadcrumb object</returns>
        Breadcrumb PageToBreadcrumb(TreeNode Page, bool IsCurrentPage);
        Task<Breadcrumb> PageToBreadcrumbAsync(TreeNode Page, bool IsCurrentPage);
    }
}