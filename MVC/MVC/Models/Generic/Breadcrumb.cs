namespace Generic.Models
{
    public class Breadcrumb
    {
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public bool IsCurrentPage { get; set; }

        public Breadcrumb() { }
    }
}