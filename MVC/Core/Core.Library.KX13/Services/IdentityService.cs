using CMS.Base;
using CMS.DataEngine;
using CMS.SiteProvider;
using System.Data;

namespace Core.Services.Implementations
{
    public class IdentityService : IIdentityService
    {
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
        private readonly ISiteService _siteService;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;
        private readonly ISiteInfoProvider _siteInfoProvider;

        public IdentityService(IProgressiveCache progressiveCache,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
            ISiteService siteService,
            ICacheRepositoryContext cacheRepositoryContext,
            ISiteInfoProvider siteInfoProvider)
        {
            _progressiveCache = progressiveCache;
            _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
            _siteService = siteService;
            _cacheRepositoryContext = cacheRepositoryContext;
            _siteInfoProvider = siteInfoProvider;
        }

        public async Task<Result<DocumentIdentity>> HydrateDocumentIdentity(DocumentIdentity identity)
        {
            // If current identity is full, then just return it.
            if (identity.DocumentGuid.HasValue && identity.DocumentId.HasValue && identity.NodeAliasPathAndMaybeCultureAndSiteId.TryGetValue(out var nodepathculture) && nodepathculture.Item2.HasValue && nodepathculture.Item3.HasValue)
            {
                return identity;
            }
            var currentSiteID = _siteService.CurrentSite.SiteID;

            // Site settings are already cached in the Provider
            var defaultCulture = SettingsKeyInfoProvider.GetValue("CMSDefaultCultureCode", currentSiteID).AsNullOrWhitespaceMaybe().GetValueOrDefault("en-US");
            var culture = _cacheRepositoryContext.CurrentCulture();

            var dictionaryResult = await GetNodeDocumentHolderAsync();
            if (dictionaryResult.TryGetValue(out var dictionary))
            {
                if (identity.DocumentId.TryGetValue(out var id) && dictionary.Document.ById.TryGetValue(id, out var newIdentityFromId))
                {
                    return newIdentityFromId;
                }
                if (identity.DocumentGuid.TryGetValue(out var guid) && dictionary.Document.ByGuid.TryGetValue(guid, out var newIdentityFromGuid))
                {
                    return newIdentityFromGuid;
                }
                if (identity.NodeAliasPathAndMaybeCultureAndSiteId.TryGetValue(out var nodePathValues))
                {
                    string key = $"{nodePathValues.Item1}|{nodePathValues.Item2.GetValueOrDefault(culture)}|{nodePathValues.Item3.GetValueOrDefault(currentSiteID)}".ToLower();
                    if (dictionary.Document.ByNodeAliasPathCultureSiteIDKey.TryGetValue(key, out var newIdentityFromPath))
                    {
                        return newIdentityFromPath;
                    }

                    string keyCultureLess = $"{nodePathValues.Item1}|{nodePathValues.Item3.GetValueOrDefault(currentSiteID)}".ToLower();
                    if (dictionary.Document.ByNodeAliasPathSiteIDKey.TryGetValue(keyCultureLess, out var newIdentityFromPathNoCulture))
                    {
                        return newIdentityFromPathNoCulture;
                    }
                }
                return Result.Failure<DocumentIdentity>("Could not find document identity.");
            }
            else
            {
                // Can't use cached version, so just generate manually
                var queryParams = new QueryDataParameters();
                var query = $"select NodeAliasPath, NodeSiteID, DocumentCulture, DocumentID, DocumentGUID from View_CMS_Tree_Joined where ";
                if (identity.DocumentId.TryGetValue(out var id))
                {
                    query += $"DocumentID = @DocumentID";
                    queryParams.Add(new DataParameter("@DocumentID", id));
                }
                if (identity.DocumentGuid.TryGetValue(out var guid))
                {
                    query += $"DocumentGuid = @DocumentGuid";
                    queryParams.Add(new DataParameter("@DocumentGuid", guid));
                }
                if (identity.NodeAliasPathAndMaybeCultureAndSiteId.TryGetValue(out var nodePathValues))
                {
                    // for documents, culture should be a preferred otherwise use current site default or first found
                    query += $"NodeAliasPath = @NodeAliasPath and NodeSiteID = @NodeSiteID order by case when DocumentCulture = @DocumentCulture then 0 else case when DocumentCulture = @DefaultCulture then 1 else 2 end end";
                    queryParams.Add(new DataParameter("@NodeAliasPath", nodePathValues.Item1));
                    queryParams.Add(new DataParameter("@DocumentCulture", nodePathValues.Item2.GetValueOrDefault(culture)));
                    queryParams.Add(new DataParameter("@DefaultCulture", nodePathValues.Item2.GetValueOrDefault(defaultCulture)));
                    queryParams.Add(new DataParameter("@NodeSiteID", nodePathValues.Item3.GetValueOrDefault(currentSiteID)));
                }
                var item = (await XperienceCommunityConnectionHelper.ExecuteQueryAsync(query, queryParams, QueryTypeEnum.SQLQuery)).Tables[0].Rows.Cast<DataRow>();
                if (item.FirstOrMaybe().TryGetValue(out var docRow))
                {
                    return new DocumentIdentity()
                    {
                        DocumentId = (int)docRow["DocumentID"],
                        DocumentGuid = (Guid)docRow["DocumentGuid"],
                        NodeAliasPathAndMaybeCultureAndSiteId = new Tuple<string, Maybe<string>, Maybe<int>>((string)docRow["NodeAliasPath"], (string)docRow["DocumentCulture"], (int)docRow["NodeSiteID"])
                    };
                }
                else
                {
                    return Result.Failure<DocumentIdentity>("Could not find a document with the given document identity");
                }
            }
        }

        public async Task<Result<NodeIdentity>> HydrateNodeIdentity(NodeIdentity identity)
        {
            // If current identity is full, then just return it.
            if (identity.NodeGuid.HasValue && identity.NodeId.HasValue && identity.NodeAliasPathAndSiteId.TryGetValue(out var nodePathSite) && nodePathSite.Item2.HasValue)
            {
                return identity;
            }

            var currentSiteID = _siteService.CurrentSite.SiteID;
            var culture = _cacheRepositoryContext.CurrentCulture();

            var dictionaryResult = await GetNodeDocumentHolderAsync();
            if (dictionaryResult.TryGetValue(out var dictionary))
            {
                if (identity.NodeId.TryGetValue(out var id) && dictionary.Node.ById.TryGetValue(id, out var newIdentityFromId))
                {
                    return newIdentityFromId;
                }
                if (identity.NodeGuid.TryGetValue(out var guid) && dictionary.Node.ByGuid.TryGetValue(guid, out var newIdentityFromGuid))
                {
                    return newIdentityFromGuid;
                }
                if (identity.NodeAliasPathAndSiteId.TryGetValue(out var nodePathValues))
                {
                    string key = $"{nodePathValues.Item1}|{nodePathValues.Item2.GetValueOrDefault(currentSiteID)}".ToLower();
                    if (dictionary.Node.ByNodeAliasPathSiteIDKey.TryGetValue(key, out var newIdentityFromPath))
                    {
                        return newIdentityFromPath;
                    }
                }
                return Result.Failure<NodeIdentity>("Could not find document identity.");
            }
            else
            {
                // Can't use cached version, so just generate manually
                var queryParams = new QueryDataParameters();
                var query = $"select NodeID, NodeGUID, NodeAliasPath, NodeSiteID from View_CMS_Tree_Joined where ";
                if (identity.NodeId.TryGetValue(out var id))
                {
                    query += $"DocumentID = @DocumentID";
                    queryParams.Add(new DataParameter("@DocumentID", id));
                }
                if (identity.NodeGuid.TryGetValue(out var guid))
                {
                    query += $"DocumentGuid = @DocumentGuid";
                    queryParams.Add(new DataParameter("@DocumentGuid", guid));
                }
                if (identity.NodeAliasPathAndSiteId.TryGetValue(out var nodePathValues))
                {
                    query += $"NodeAliasPath = @NodeAliasPath and NodeSiteID = @NodeSiteID";
                    queryParams.Add(new DataParameter("@NodeAliasPath", nodePathValues.Item1));
                    queryParams.Add(new DataParameter("@NodeSiteID", nodePathValues.Item2.GetValueOrDefault(currentSiteID)));
                }
                var item = (await XperienceCommunityConnectionHelper.ExecuteQueryAsync(query, queryParams, QueryTypeEnum.SQLQuery)).Tables[0].Rows.Cast<DataRow>();
                if (item.FirstOrMaybe().TryGetValue(out var docRow))
                {
                    return new NodeIdentity()
                    {
                        NodeId = (int)docRow["DocumentID"],
                        NodeGuid = (Guid)docRow["DocumentGuid"],
                        NodeAliasPathAndSiteId = new Tuple<string, Maybe<int>>((string)docRow["NodeAliasPath"], (int)docRow["NodeSiteID"])
                    };
                }
                else
                {
                    return Result.Failure<NodeIdentity>("Could not find a node with the given node identity");
                }
            }
        }

        public async Task<Result<ObjectIdentity>> HydrateObjectIdentity(ObjectIdentity identity, string className)
        {
            if (identity.Id.HasNoValue && identity.Guid.HasNoValue && identity.CodeName.HasNoValue)
            {
                return Result.Failure<ObjectIdentity>("No identities provided from the given identity, cannot parse.");
            }

            var classInfo = _progressiveCache.Load(cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = _cacheDependencyBuilderFactory.Create(false).ObjectType(DataClassInfo.OBJECT_TYPE).GetCMSCacheDependency();
                }
                return DataClassInfoProvider.GetDataClassInfo(className);
            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(), "GetClassInfoForHydration", className));

            if (classInfo == null)
            {
                return Result.Failure<ObjectIdentity>($"Class of {className} not found.");
            }

            var classObj = (new InfoObjectFactory(className).Singleton);
            if (classObj is not BaseInfo baseClassObj)
            {
                return Result.Failure<ObjectIdentity>($"Class of {className} not a BaseInfo typed class, cannot parse.");
            }

            var dictionaryResult = await GetObjectIdentitiesAsync(classInfo, baseClassObj);
            if (dictionaryResult.TryGetValue(out var dictionary))
            {
                if (identity.Id.TryGetValue(out var idVal) && dictionary.ById.TryGetValue(idVal, out var objectIdentityFromId))
                {
                    return objectIdentityFromId;
                }
                if (identity.CodeName.TryGetValue(out var codeNameVal) && dictionary.ByCodeName.TryGetValue(codeNameVal.ToLower(), out var objectIdentitFromCodeName))
                {
                    return objectIdentitFromCodeName;
                }
                if (identity.Guid.TryGetValue(out var guidVal) && dictionary.ByGuid.TryGetValue(guidVal, out var objectIdentityFromGuid))
                {
                    return objectIdentityFromGuid;
                }
                return Result.Failure<ObjectIdentity>($"Could not find any matching {className} objects for any of the identity's values:  [{identity.Id.GetValueOrDefault(0)}-{identity.CodeName.GetValueOrDefault(string.Empty)}-{identity.Guid.GetValueOrDefault(Guid.Empty)}]");
            }
            else
            {
                // Dependencies are touching too much, use non cached version
                string query = GetObjectIdentitySelectStatement(classInfo, baseClassObj);
                var typeInfo = baseClassObj.TypeInfo;
                var idColumnMaybe = typeInfo.IDColumn.AsNullOrWhitespaceMaybe();
                var guidColumnMaybe = typeInfo.GUIDColumn.AsNullOrWhitespaceMaybe();
                var codeNameColumnMaybe = typeInfo.CodeNameColumn.AsNullOrWhitespaceMaybe();
                var queryParams = new QueryDataParameters();
                if (identity.Id.TryGetValue(out var id) && idColumnMaybe.TryGetValue(out var idColumn))
                {
                    query += $"[{idColumn}] = @ID";
                    queryParams.Add(new DataParameter("@ID", id));
                }
                if (identity.Guid.TryGetValue(out var guid) && guidColumnMaybe.TryGetValue(out var guidColumn))
                {
                    query += $"[{guidColumn}] = @Guid";
                    queryParams.Add(new DataParameter("@Guid", guid));
                }
                if (identity.CodeName.TryGetValue(out var codeName) && codeNameColumnMaybe.TryGetValue(out var codeNameColumn))
                {
                    query += $"[{codeNameColumn}] = @CodeName";
                    queryParams.Add(new DataParameter("@CodeName", codeName));
                }
                var item = (await XperienceCommunityConnectionHelper.ExecuteQueryAsync(query, queryParams, QueryTypeEnum.SQLQuery)).Tables[0].Rows.Cast<DataRow>();
                if (item.FirstOrMaybe().TryGetValue(out var objRow))
                {
                    var newIdentity = new ObjectIdentity();
                    if (idColumnMaybe.TryGetValue(out var idColumnVal))
                    {
                        newIdentity.Id = ValidationHelper.GetInteger(objRow[idColumnVal], 0).WithMatchAsNone(0);
                    }
                    if (guidColumnMaybe.TryGetValue(out var guidColumnVal))
                    {
                        newIdentity.Guid = ValidationHelper.GetGuid(objRow[guidColumnVal], Guid.Empty).WithMatchAsNone(Guid.Empty);
                    }
                    if (codeNameColumnMaybe.TryGetValue(out var codeNameColumnVal))
                    {
                        newIdentity.CodeName = ValidationHelper.GetString(objRow[codeNameColumnVal], string.Empty).WithMatchAsNone(string.Empty);
                    }
                    return newIdentity;
                }
                else
                {
                    return Result.Failure<ObjectIdentity>($"Could not find an object of type {className} with the given identity [{identity.Id.GetValueOrDefault(0)}-{identity.CodeName.GetValueOrDefault(string.Empty)}-{identity.Guid.GetValueOrDefault(Guid.Empty)}]");
                }
            }
        }

        private string GetObjectIdentitySelectStatement(DataClassInfo classInfo, BaseInfo baseClassObj)
        {
            var typeInfo = baseClassObj.TypeInfo;
            return $"select {typeInfo.IDColumn} {(typeInfo.GUIDColumn.AsNullOrWhitespaceMaybe().TryGetValue(out var guidColumn) ? $", [{guidColumn}]" : "")} {(typeInfo.CodeNameColumn.AsNullOrWhitespaceMaybe().TryGetValue(out var codenameColumn) ? $", [{codenameColumn}]" : "")} from {classInfo.ClassTableName}";
        }

        private async Task<Result<ObjectIdentityDictionaries>> GetObjectIdentitiesAsync(DataClassInfo classInfo, BaseInfo baseClassObj)
        {
            var builder = _cacheDependencyBuilderFactory.Create(false).ObjectType(baseClassObj.TypeInfo.ObjectClassName);
            if (!builder.DependenciesNotTouchedSince(TimeSpan.FromSeconds(30)))
            {
                return Result.Failure<ObjectIdentityDictionaries>("Dependency recently touched, waiting 30 seconds before using cached version.");
            }


            return await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                var allItems = (await XperienceCommunityConnectionHelper.ExecuteQueryAsync(GetObjectIdentitySelectStatement(classInfo, baseClassObj), new QueryDataParameters(), QueryTypeEnum.SQLQuery))
                .Tables[0].Rows.Cast<DataRow>();

                var dictionary = new ObjectIdentityDictionaries();
                foreach (DataRow item in allItems)
                {
                    // Create
                    var objectIdentity = new ObjectIdentity()
                    {
                        Id = (int)item[baseClassObj.TypeInfo.IDColumn]
                    };
                    if (baseClassObj.TypeInfo.GUIDColumn.AsNullOrWhitespaceMaybe().TryGetValue(out var guidColumn))
                    {
                        objectIdentity.Guid = ValidationHelper.GetGuid(item[guidColumn], Guid.Empty).WithMatchAsNone(Guid.Empty);
                    }
                    if (baseClassObj.TypeInfo.CodeNameColumn.AsNullOrWhitespaceMaybe().TryGetValue(out var codeNameColumn))
                    {
                        objectIdentity.CodeName = ValidationHelper.GetString(item[codeNameColumn], string.Empty).WithMatchAsNone(string.Empty);
                    }

                    // Add
                    if (objectIdentity.Id.TryGetValue(out var idVal))
                    {
                        dictionary.ById.TryAdd(idVal, objectIdentity);
                    }
                    if (objectIdentity.Guid.TryGetValue(out var guidVal))
                    {
                        dictionary.ByGuid.TryAdd(guidVal, objectIdentity);
                    }
                    if (objectIdentity.CodeName.TryGetValue(out var codeNameVal))
                    {
                        dictionary.ByCodeName.TryAdd(codeNameVal, objectIdentity);
                    }
                }
                return dictionary;

            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(), "GetAllObjectIdentity", baseClassObj.TypeInfo.ObjectClassName));

        }

        private async Task<Result<NodeDocumentIdentityHolder>> GetNodeDocumentHolderAsync()
        {

            var keys = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = _cacheDependencyBuilderFactory.Create(false).ObjectType(SiteInfo.OBJECT_TYPE).GetCMSCacheDependency();
                }
                return (await _siteInfoProvider.Get()
                 .Columns(nameof(SiteInfo.SiteName))
                .GetEnumerableTypedResultAsync()
                )
                .Select(x => x.SiteName);
            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(), "GetAllSiteNames"));

            var builder = _cacheDependencyBuilderFactory.Create(false).AddKeys(keys);

            if (!builder.DependenciesNotTouchedSince(TimeSpan.FromSeconds(30)))
            {
                return Result.Failure<NodeDocumentIdentityHolder>("Dependency recently touched, waiting 30 seconds before using cached version.");
            }

            return await _progressiveCache.LoadAsync(async cs =>
            {

                if (cs.Cached)
                {
                    cs.CacheDependency = builder
                        .GetCMSCacheDependency();
                }

                var query = $"select NodeID, NodeGUID, NodeAliasPath, NodeSiteID, DocumentCulture, DocumentID, DocumentGUID from View_CMS_Tree_Joined";
                var allPages = (await XperienceCommunityConnectionHelper.ExecuteQueryAsync(query, new QueryDataParameters(), QueryTypeEnum.SQLQuery)).Tables[0].Rows.Cast<DataRow>()
                    .Select(x => (NodeID: (int)x["NodeID"], NodeGuid: (Guid)x["NodeGuid"], NodeAliasPath: (string)x["NodeAliasPath"], NodeSiteID: (int)x["NodeSiteID"], DocumentCulture: (string)x["DocumentCulture"], DocumentID: (int)x["DocumentID"], DocumentGUID: (Guid)x["DocumentGUID"]));

                var nodeIdentityDictionaries = new NodeIdentityDictionaries();
                var documentIdentityDictionaries = new DocumentIdentityDictionaries();
                foreach (var nodeGrouping in allPages.GroupBy(x => x.NodeID))
                {
                    var firstItem = nodeGrouping.First();
                    var nodeIdentity = new NodeIdentity()
                    {
                        NodeId = firstItem.NodeID,
                        NodeGuid = firstItem.NodeGuid,
                        NodeAliasPathAndSiteId = new Tuple<string, Maybe<int>>(firstItem.NodeAliasPath, firstItem.NodeSiteID)
                    };
                    var nodeKey = $"{firstItem.NodeAliasPath}|{firstItem.NodeSiteID}".ToLower();
                    nodeIdentityDictionaries.ById.TryAdd(firstItem.NodeID, nodeIdentity);
                    nodeIdentityDictionaries.ByNodeAliasPathSiteIDKey.TryAdd(nodeKey, nodeIdentity);
                    nodeIdentityDictionaries.ByGuid.TryAdd(firstItem.NodeGuid, nodeIdentity);

                    foreach (var document in nodeGrouping)
                    {
                        var documentIdentity = new DocumentIdentity()
                        {
                            DocumentId = document.DocumentID,
                            DocumentGuid = document.DocumentGUID,
                            NodeAliasPathAndMaybeCultureAndSiteId = new Tuple<string, Maybe<string>, Maybe<int>>(document.NodeAliasPath, document.DocumentCulture, document.NodeSiteID)
                        };
                        var documentKey = $"{document.NodeAliasPath}|{document.DocumentCulture}|{document.NodeSiteID}".ToLower();
                        var documentCulturelessKey = $"{document.NodeAliasPath}|{document.NodeSiteID}".ToLower();
                        documentIdentityDictionaries.ById.TryAdd(document.DocumentID, documentIdentity);
                        documentIdentityDictionaries.ByGuid.TryAdd(document.DocumentGUID, documentIdentity);
                        documentIdentityDictionaries.ByNodeAliasPathCultureSiteIDKey.TryAdd(documentKey, documentIdentity);
                        documentIdentityDictionaries.ByNodeAliasPathSiteIDKey.TryAdd(documentCulturelessKey, documentIdentity);
                    }
                }
                return new NodeDocumentIdentityHolder(documentIdentityDictionaries, nodeIdentityDictionaries);

            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(), "GetAllNodeDocumentIdenties"));

        }
    }

    public class NodeDocumentIdentityHolder
    {
        public NodeDocumentIdentityHolder(DocumentIdentityDictionaries document, NodeIdentityDictionaries node)
        {
            Document = document;
            Node = node;
        }

        public DocumentIdentityDictionaries Document { get; set; }
        public NodeIdentityDictionaries Node { get; set; }
    }

    public class DocumentIdentityDictionaries
    {
        public Dictionary<int, DocumentIdentity> ById { get; set; } = new Dictionary<int, DocumentIdentity>();
        public Dictionary<string, DocumentIdentity> ByNodeAliasPathCultureSiteIDKey { get; set; } = new Dictionary<string, DocumentIdentity>();
        public Dictionary<string, DocumentIdentity> ByNodeAliasPathSiteIDKey { get; set; } = new Dictionary<string, DocumentIdentity>();

        public Dictionary<Guid, DocumentIdentity> ByGuid { get; set; } = new Dictionary<Guid, DocumentIdentity>();
    }

    public class NodeIdentityDictionaries
    {
        public Dictionary<int, NodeIdentity> ById { get; set; } = new Dictionary<int, NodeIdentity>();
        public Dictionary<string, NodeIdentity> ByNodeAliasPathSiteIDKey { get; set; } = new Dictionary<string, NodeIdentity>();
        public Dictionary<Guid, NodeIdentity> ByGuid { get; set; } = new Dictionary<Guid, NodeIdentity>();
    }

    public class ObjectIdentityDictionaries
    {
        public Dictionary<int, ObjectIdentity> ById { get; set; } = new Dictionary<int, ObjectIdentity>();
        public Dictionary<string, ObjectIdentity> ByCodeName { get; set; } = new Dictionary<string, ObjectIdentity>();
        public Dictionary<Guid, ObjectIdentity> ByGuid { get; set; } = new Dictionary<Guid, ObjectIdentity>();
    }
}
