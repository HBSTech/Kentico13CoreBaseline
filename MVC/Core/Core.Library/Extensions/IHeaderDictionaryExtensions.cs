using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http
{
    public static class IHeaderDictionaryExtensions
    {
        public static void AddOrReplace(this IHeaderDictionary dictionary, string key, StringValues value)
        {
            if(dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            } else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
