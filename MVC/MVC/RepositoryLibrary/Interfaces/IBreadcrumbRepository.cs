using Generic.Models;
using MVCCaching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IBreadcrumbRepository : IRepository
    {
        /// <summary>
        /// Gets a list of Breadcrumbs
        /// </summary>
        /// <param name="PageIdentifier">The Page Identifier (NodeID)</param>
        /// <returns></returns>
        List<Breadcrumb> GetBreadcrumbs(int PageIdentifier, bool IncludeDefaultBreadcrumb = true);
        Task<List<Breadcrumb>> GetBreadcrumbsAsync(int PageIdentifier, bool IncludeDefaultBreadcrumb = true);

        /// <summary>
        /// Gets the Default Breadcrumb (built from Resource Strings)
        /// </summary>
        /// <returns></returns>
        Breadcrumb GetDefaultBreadcrumb();
        Task<Breadcrumb> GetDefaultBreadcrumbAsync();

        /// <summary>
        /// Converts Breadcrumbs into a list of JsonLD items
        /// </summary>
        /// <param name="Breadcrumbs"></param>
        /// <returns></returns>
        BreadcrumbJsonLD BreadcrumbsToJsonLD(IEnumerable<Breadcrumb> Breadcrumbs, bool ExcludeFirst = true);
        Task<BreadcrumbJsonLD> BreadcrumbsToJsonLDAsync(IEnumerable<Breadcrumb> Breadcrumbs, bool ExcludeFirst = true);
    }
}