using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using MVCCaching.Base.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// These extension methods allow you to append the CacheDependencyStore for IPageRetreiver calls
    /// </summary>
    public static class IPageCacheDependencyBuilderExtensions
    {
        public static IPageCacheDependencyBuilder<TPageType> Custom<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, IEnumerable<string> dependencies, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            cacheDependenciesStore.Store(dependencies.ToArray());
            foreach (string dependency in dependencies)
            {
                pageCacheDependencyBuilder.Custom(dependency);
            }
            return pageCacheDependencyBuilder;
        }

        public static IPageCacheDependencyBuilder<TPageType> Custom<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, string key, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            cacheDependenciesStore.Store(new string[] { key });
            return pageCacheDependencyBuilder.Custom(key);
        }

        public static IPageCacheDependencyBuilder<TPageType> Objects<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, IEnumerable<BaseInfo> objects, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            cacheDependenciesStore.Store(objects.Select(o => CacheHelper.GetCacheItemName(null, o.TypeInfo.ObjectType, "byid", o.Generalized.ObjectID)).ToArray());
            return pageCacheDependencyBuilder.Objects(objects);
        }

        public static IPageCacheDependencyBuilder<TPageType> ObjectType<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, string objectType, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            if (objectType.Any())
            {
                cacheDependenciesStore.Store(new string[] { CacheHelper.GetCacheItemName(null, objectType, "all") });
                return pageCacheDependencyBuilder.ObjectType(objectType);
            }
            return pageCacheDependencyBuilder;
        }

        public static IPageCacheDependencyBuilder<TPageType> PageOrder<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            cacheDependenciesStore.Store(new string[] { CacheHelper.GetCacheItemName(null, "nodeorder") });
            return pageCacheDependencyBuilder.PageOrder();
        }

        public static IPageCacheDependencyBuilder<TPageType> PagePath<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, string path, string siteName, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            return pageCacheDependencyBuilder.PagePath(path, siteName, PathTypeEnum.Explicit, cacheDependenciesStore);
        }

        public static IPageCacheDependencyBuilder<TPageType> PagePath<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, string path, string siteName, PathTypeEnum type, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            cacheDependenciesStore.Store(GetPathDependencyCacheKeys(siteName, path, type).ToArray());
            return pageCacheDependencyBuilder.PagePath(path, type);
        }

        public static IPageCacheDependencyBuilder<TPageType> Pages<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, IEnumerable<TreeNode> pages, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            if (pages.Any())
            {
                cacheDependenciesStore.Store(pages.Select(p => CacheHelper.GetCacheItemName(null, "documentid", p.DocumentID)).ToArray());
                return pageCacheDependencyBuilder.Pages(pages);
            }
            return pageCacheDependencyBuilder;
        }

        public static IPageCacheDependencyBuilder<TPageType> Pages<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, string className, string siteName, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            cacheDependenciesStore.Store(new string[] { CacheHelper.GetCacheItemName(null, "nodes", siteName, className, "all")});
            return pageCacheDependencyBuilder.Pages(className);
        }

        public static IPageCacheDependencyBuilder<TPageType> PageType<TPageType>(this IPageCacheDependencyBuilder<TPageType> pageCacheDependencyBuilder, string className, ICacheDependenciesStore cacheDependenciesStore) where TPageType : TreeNode, new()
        {
            cacheDependenciesStore.Store(new string[] { CacheHelper.GetCacheItemName(null, DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE, "byname", className) });
            return pageCacheDependencyBuilder.PageType(className);
        }

        private static List<string> GetPathDependencyCacheKeys(string siteName, string path, PathTypeEnum type)
        {
            var dependencies = new List<string>();

            switch (type)
            {
                case PathTypeEnum.Single:
                    dependencies.Add(CacheHelper.GetCacheItemName(null, "node", siteName, path));
                    break;

                case PathTypeEnum.Children:
                    dependencies.Add(CacheHelper.GetCacheItemName(null, "node", siteName, path, "childnodes"));
                    break;

                case PathTypeEnum.Section:
                    dependencies.Add(CacheHelper.GetCacheItemName(null, "node", siteName, path, "childnodes"));
                    dependencies.Add(CacheHelper.GetCacheItemName(null, "node", siteName, path));
                    break;

                case PathTypeEnum.Explicit:
                default:
                    if (path.EndsWithCSafe("/%"))
                    {
                        dependencies.Add(CacheHelper.GetCacheItemName(null, "node", siteName, TreePathUtils.GetParentPath(path)));
                    }
                    else
                    {
                        dependencies.Add(CacheHelper.GetCacheItemName(null, "node", siteName, path));
                    }
                    break;
            }

            return dependencies;
        }

    }
}
