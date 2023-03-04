using MVCCaching;

namespace Core.Services.Implementations
{
    [AutoDependencyInjection]
    public class PageIdentityFactory : IPageIdentityFactory
    {

        public PageIdentityFactory()
        {
        }

        public PageIdentity<TData> Convert<TData, TOriginalData>(PageIdentity<TOriginalData> pageIdentity, Func<TOriginalData, TData> conversion)
        {
            TData data = conversion.Invoke(pageIdentity.Data);
            return new PageIdentity<TData>(data, pageIdentity);
        }
    }
}
