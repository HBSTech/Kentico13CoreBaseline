namespace Navigation.Components.Navigation.BreadcrumbsJson
{
    public record BreadcrumbsJsonViewModel
    {
        public BreadcrumbsJsonViewModel(string serializedBreadcrumbJsonLD)
        {
            SerializedBreadcrumbJsonLD = serializedBreadcrumbJsonLD;
        }

        public string SerializedBreadcrumbJsonLD { get; set; }
    }
}
