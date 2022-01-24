using Generic.Models;
using MVCCaching;

namespace Generic.Services.Interfaces
{
    public interface IPageIdentityFactory : IService
    {
        PageIdentity<TData> Convert<TData, TOriginalData>(PageIdentity<TOriginalData> pageIdentity);
    }
}
