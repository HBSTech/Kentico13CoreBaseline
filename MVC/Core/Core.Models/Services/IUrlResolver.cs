namespace Core.Services
{
    public interface IUrlResolver
    {
        public string GetAbsoluteUrl(string relativeUrl);

        public string ResolveUrl(string url);
    }
}
