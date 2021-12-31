
using MVCCaching;

namespace Generic.Services.Interfaces
{
    public interface IUrlResolver : IService
    {

        public string GetAbsoluteUrl(string relativeUrl);

        public string ResolveUrl(string url);
    }
}
