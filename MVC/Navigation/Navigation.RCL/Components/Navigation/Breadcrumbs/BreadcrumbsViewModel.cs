namespace Navigation.Components.Navigation.Breadcrumbs
{
    public record BreadcrumbsViewModel
    {
        public IEnumerable<Breadcrumb> Breadcrumbs { get; set; } = Array.Empty<Breadcrumb>();
    }
}
