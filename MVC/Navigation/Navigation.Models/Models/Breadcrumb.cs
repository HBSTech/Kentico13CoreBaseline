namespace Navigation.Models
{
    public class Breadcrumb
    {
        public Breadcrumb(string linkText, string linkUrl)
        {
            LinkText = linkText;
            LinkUrl = linkUrl;
        }
        public Breadcrumb(string linkText, string linkUrl, bool isCurrentPage)
        {
            LinkText = linkText;
            LinkUrl = linkUrl;
            IsCurrentPage = isCurrentPage;
        }

        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public bool IsCurrentPage { get; set; } = false;

        
    }
}