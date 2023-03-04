namespace Navigation.Repositories
{
    public interface IBreadcrumbRepository
    {
        /// <summary>
        /// Gets a list of Breadcrumbs
        /// </summary>
        /// <param name="nodeID">The Page Identifier (NodeID)</param>
        /// <returns></returns>
        Task<List<Breadcrumb>> GetBreadcrumbsAsync(int nodeID, bool includeDefaultBreadcrumb = true);

        /// <summary>
        /// Gets the Default Breadcrumb (built from Resource Strings)
        /// </summary>
        /// <returns></returns>
        Task<Breadcrumb> GetDefaultBreadcrumbAsync();

        /// <summary>
        /// Converts Breadcrumbs into a list of JsonLD items
        /// </summary>
        /// <param name="breadcrumbs"></param>
        /// <returns></returns>
        Task<BreadcrumbJsonLD> BreadcrumbsToJsonLDAsync(IEnumerable<Breadcrumb> breadcrumbs, bool excludeFirst = true);
    }
}