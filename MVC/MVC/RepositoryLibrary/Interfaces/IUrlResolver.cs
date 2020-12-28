
using MVCCaching;

namespace Generic.Repositories.Interfaces
{
    public interface IUrlResolver : IService
    {

        public string GetAbsoluteUrl(string RelativeUrl);

        public string ResolveUrl(string Url);
    }
}
