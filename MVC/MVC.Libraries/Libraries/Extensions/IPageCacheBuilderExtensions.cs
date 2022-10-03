using CMS.DocumentEngine;
using System;
using System.Collections.Generic;

namespace Kentico.Content.Web.Mvc
{
    public static class IPageCacheBuilderExtensions
    {
        /// <summary>
        /// Helper to simplify the configuration process.
        /// </summary>
        /// <typeparam name="TPageType"></typeparam>
        /// <param name="cs">The CacheBuilder</param>
        /// <param name="builder">The CacheDependencyKeysBuilder with it's dependencies</param>
        /// <param name="expirationMinutes">The Time for it to expire</param>
        /// <param name="keys">All keys, can accept IEnumerable, ICacheKey, and standard objects (which will return the .ToString(), which if you need a custom key for it then implement ICacheKey on the object class)</param>
        /// <returns></returns>
        public static IPageCacheBuilder<TPageType> Configure<TPageType>(this IPageCacheBuilder<TPageType> cs, CacheDependencyKeysBuilder builder, double expirationMinutes, params object[] keys) where TPageType : TreeNode, new()
        {
            string keyName = (keys != null && keys.Length > 0 ? string.Join("|", keys.Cast<object>().Select(x =>
            {
                if (x == null)
                {
                    return string.Empty;
                }
                else if (x is ICacheKey)
                {
                    return ((ICacheKey)x).GetCacheKey();
                }
                else if (x is IEnumerable<object>)
                {
                    return string.Join("|", ((IEnumerable<object>)x).Select(y => (y is ICacheKey ? ((ICacheKey)y)?.GetCacheKey() ?? string.Empty : y?.ToString() ?? string.Empty)));
                }
                else
                {
                    return x.ToString();
                }
            })) : Guid.NewGuid().ToString());
            return cs.Key(keyName)
        .Dependencies((items, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
        .Expiration(TimeSpan.FromMinutes(expirationMinutes));
        }

    }
}
