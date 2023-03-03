namespace Core.Services
{
    public interface IPageIdentityFactory
    {
        /// <summary>
        /// Converts the PageIdentity's Data from one type to another using the given conversion method
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TOriginalData"></typeparam>
        /// <param name="pageIdentity"></param>
        /// <param name="conversion"></param>
        /// <returns></returns>
        PageIdentity<TData> Convert<TData, TOriginalData>(PageIdentity<TOriginalData> pageIdentity, Func<TOriginalData, TData> conversion);
    }
}
