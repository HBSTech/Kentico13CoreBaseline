using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using Generic.Repositories.Interfaces;
using MVCCaching.Base.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Libraries.Helpers
{
    /// <summary>
    /// Helper Method that allows you to store cache dependencies easily, and also be able to apply them to the IPageRetriever, should NOT inject this but instead declare it since otherwise the cacheKeys will be the same each time.
    /// </summary>
    public class CacheDependencyKeysBuilder
    {
        private readonly HashSet<string> cacheKeys = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly ISiteRepository context;
        private readonly ICacheDependenciesStore cacheDependenciesStore;

        public CacheDependencyKeysBuilder(ISiteRepository context, ICacheDependenciesStore cacheDependenciesStore)
        {
            this.context = context;
            this.cacheDependenciesStore = cacheDependenciesStore;
        }

        public ISet<string> GetKeys() => cacheKeys;

        private void Add(string key)
        {
            cacheKeys.Add(key);
            cacheDependenciesStore.Store(new string[] { key });
        }

        private void UnionWith(IEnumerable<string> keys)
        {
            cacheKeys.UnionWith(keys);
            cacheDependenciesStore.Store(keys.ToArray());
        }

        public CacheDependencyKeysBuilder CustomKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return this;
            }

            Add(key);

            return this;
        }
        public CacheDependencyKeysBuilder CustomKeys(IEnumerable<string> keys)
        {
            UnionWith(keys);

            return this;
        }

        public CacheDependencyKeysBuilder PageTypeDefinition(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return this;
            }

            Add($"{DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE}|byname|{className}");

            return this;
        }

        public CacheDependencyKeysBuilder ObjectType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return this;
            }

            Add($"{typeName}|all");

            return this;
        }

        public CacheDependencyKeysBuilder PageOrder()
        {
            Add("nodeorder");

            return this;
        }

        public CacheDependencyKeysBuilder SitePageType(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return this;
            }

            Add($"nodes|{context.CurrentSiteName()}|{className}|all");

            return this;
        }

        public CacheDependencyKeysBuilder Pages(IEnumerable<TreeNode> pages)
        {
            UnionWith(pages.Select(p => $"documentid|{p.DocumentID}"));

            return this;
        }

        public CacheDependencyKeysBuilder Object(string objectType, int? id) => Object(objectType, id.GetValueOrDefault());
        public CacheDependencyKeysBuilder Object(string objectType, int id)
        {
            if (string.IsNullOrWhiteSpace(objectType) || id <= 0)
            {
                return this;
            }

            Add($"{objectType}|byid|{id}");

            return this;
        }

        public CacheDependencyKeysBuilder Object(string objectType, Guid? guid) => Object(objectType, guid.GetValueOrDefault());
        public CacheDependencyKeysBuilder Object(string objectType, Guid guid)
        {
            if (guid == default)
            {
                return this;
            }

            Add($"{objectType}|byguid|{guid}");

            return this;
        }

        public CacheDependencyKeysBuilder Object(string objectType, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return this;
            }

            Add($"{objectType}|byname|{name}");

            return this;
        }

        public CacheDependencyKeysBuilder Objects(IEnumerable<BaseInfo> objects)
        {
            UnionWith(objects.Select(o => $"{o.TypeInfo.ObjectType}|byid|{o.Generalized.ObjectID}"));

            return this;
        }

        public CacheDependencyKeysBuilder PagePath(string path, PathTypeEnum type = PathTypeEnum.Explicit)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return this;
            }

            switch (type)
            {
                case PathTypeEnum.Single:
                    Add($"node|{context.CurrentSiteName()}|{path}");
                    break;
                case PathTypeEnum.Children:
                    Add($"node|{context.CurrentSiteName()}|{path}|childnodes");
                    break;
                case PathTypeEnum.Section:
                    Add($"node|{context.CurrentSiteName()}|{path}");
                    Add($"node|{context.CurrentSiteName()}|{path}|childnodes");
                    break;
                case PathTypeEnum.Explicit:
                default:
                    if (path.EndsWith("/%"))
                    {
                        Add($"node|{context.CurrentSiteName()}|{path}|childnodes");
                    }
                    else
                    {
                        Add($"node|{context.CurrentSiteName()}|{path}");
                    }
                    break;
            }

            return this;
        }

        public CacheDependencyKeysBuilder Page(int? documentId) => Page(documentId.GetValueOrDefault());
        public CacheDependencyKeysBuilder Page(int documentId)
        {
            if (documentId <= 0)
            {
                return this;
            }

            Add($"documentid|{documentId}");

            return this;
        }

        public CacheDependencyKeysBuilder Pages(IEnumerable<int?> documentIds) => Pages(documentIds.Where(x => x.HasValue).Select(x => x.Value));
        public CacheDependencyKeysBuilder Pages(IEnumerable<int> documentIds)
        {
            UnionWith(documentIds.Select(id => $"documentid|{id}"));

            return this;
        }

        public CacheDependencyKeysBuilder Node(int? nodeId) => Node(nodeId.GetValueOrDefault());
        public CacheDependencyKeysBuilder Node(int nodeId)
        {
            if (nodeId <= 0)
            {
                return this;
            }

            Add($"nodeid|{nodeId}");

            return this;
        }

        public CacheDependencyKeysBuilder Node(Guid? nodeGUID) => Node(nodeGUID.GetValueOrDefault());
        public CacheDependencyKeysBuilder Node(Guid nodeGUID)
        {
            if (nodeGUID == default)
            {
                return this;
            }

            Add($"nodeguid|{context.CurrentSiteName()}|{nodeGUID}");

            return this;
        }

        public CacheDependencyKeysBuilder NodeRelationships(int? nodeId) => NodeRelationships(nodeId.GetValueOrDefault());
        public CacheDependencyKeysBuilder NodeRelationships(int nodeId)
        {
            if (nodeId <= 0)
            {
                return this;
            }

            Add($"nodeid|{nodeId}|relationships");

            return this;
        }

        public CacheDependencyKeysBuilder Page(Guid? documentGUID) => Page(documentGUID.GetValueOrDefault());
        public CacheDependencyKeysBuilder Page(Guid documentGUID)
        {
            if (documentGUID == default)
            {
                return this;
            }

            Add($"documentguid|{context.CurrentSiteName()}|{documentGUID}");

            return this;
        }

        public CacheDependencyKeysBuilder Attachment(Guid? attachmentGUID) => Attachment(attachmentGUID.GetValueOrDefault());
        public CacheDependencyKeysBuilder Attachment(Guid attachmentGUID)
        {
            if (attachmentGUID == default)
            {
                return this;
            }

            Add($"attachment|{attachmentGUID}");

            return this;
        }

        public CacheDependencyKeysBuilder Media(Guid? mediaFileGUID) => Media(mediaFileGUID.GetValueOrDefault());
        public CacheDependencyKeysBuilder Media(Guid mediaFileGUID)
        {
            if (mediaFileGUID == default)
            {
                return this;
            }

            Add($"mediafile|{mediaFileGUID}");

            return this;
        }

        public CacheDependencyKeysBuilder Collection<T>(IEnumerable<T> items, Action<T, CacheDependencyKeysBuilder> action)
        {
            foreach (var item in items.Where(x => x != null))
            {
                action(item, this);
            }

            return this;
        }


        public CacheDependencyKeysBuilder Collection<T>(IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items.Where(x => x != null))
            {
                action(item);
            }

            return this;
        }

        public CacheDependencyKeysBuilder CustomTableItems(string codeName)
        {
            if (string.IsNullOrWhiteSpace(codeName))
            {
                return this;
            }

            Add($"customtableitem.{codeName}|all");

            return this;
        }

        public CacheDependencyKeysBuilder CustomTableItem(string codeName, int? customTableItemId) => CustomTableItem(codeName, customTableItemId.GetValueOrDefault());
        public CacheDependencyKeysBuilder CustomTableItem(string codeName, int customTableItemId)
        {
            if (string.IsNullOrWhiteSpace(codeName) || customTableItemId <= 0)
            {
                return this;
            }

            Add($"customtableitem.{codeName}|byid|{customTableItemId}");

            return this;
        }

        public CacheDependencyKeysBuilder ApplyDependenciesTo(Action<string> action)
        {
            foreach (string cacheKey in cacheKeys)
            {
                action(cacheKey);
            }
            return this;
        }

        public CacheDependencyKeysBuilder ApplyAllDependenciesTo(Action<string[]> action)
        {
            action(cacheKeys.ToArray());
            return this;
        }

        public CacheDependencyKeysBuilder Clear()
        {
            cacheKeys.Clear();
            return this;
        }

        public CMSCacheDependency GetCMSCacheDependency()
        {
            return CacheHelper.GetCacheDependency(cacheKeys.ToArray());
        }
    }
}