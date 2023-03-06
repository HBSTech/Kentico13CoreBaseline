using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;
using CMS.Relationships;

public static class ObjectQueryExtensions
{
    public static DocumentQuery ColumnsNullHandled(this DocumentQuery baseQuery, string[] Columns)
    {
        if (Columns == null)
        {
            return baseQuery;
        }
        else
        {
            return baseQuery.Columns(Columns);
        }
    }

    public static DocumentQuery<TDocument> ColumnsNullHandled<TDocument>(this DocumentQuery<TDocument> baseQuery, string[] Columns) where TDocument : TreeNode, new()
    {
        if (Columns == null)
        {
            return baseQuery;
        }
        else
        {
            return baseQuery.Columns(Columns);
        }
    }

    public static MultiDocumentQuery ColumnsNullHandled(this MultiDocumentQuery baseQuery, string[] Columns)
    {
        if (Columns == null)
        {
            return baseQuery;
        }
        else
        {
            return baseQuery.Columns(Columns);
        }
    }

    public static ObjectQuery ColumnsNullHandled(this ObjectQuery baseQuery, string[] Columns)
    {
        if (Columns == null)
        {
            return baseQuery;
        }
        else
        {
            return baseQuery.Columns(Columns);
        }
    }

    public static ObjectQuery<TObject> ColumnsNullHandled<TObject>(this ObjectQuery<TObject> baseQuery, string[] Columns) where TObject : BaseInfo, new()
    {
        if (Columns == null)
        {
            return baseQuery;
        }
        else
        {
            return baseQuery.Columns(Columns);
        }
    }


    public static DocumentQuery IncludePageIdentityColumns(this DocumentQuery baseQuery)
    {
        return baseQuery.Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentCulture),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel),
                        nameof(TreeNode.NodeSiteID)
                        })
                    .WithPageUrlPaths();
    }

    public static DocumentQuery<TDocument> IncludePageIdentityColumns<TDocument>(this DocumentQuery<TDocument> baseQuery) where TDocument : TreeNode, new()
    {
        return baseQuery.Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentCulture),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel),
                        nameof(TreeNode.NodeSiteID)
                        })
                    .WithPageUrlPaths(); ;
    }

    public static MultiDocumentQuery IncludePageIdentityColumns(this MultiDocumentQuery baseQuery)
    {
        return baseQuery.Columns(new string[] {
                        nameof(TreeNode.NodeID),
                        nameof(TreeNode.DocumentID),
                        nameof(TreeNode.NodeGUID),
                        nameof(TreeNode.DocumentGUID),
                        nameof(TreeNode.NodeAlias),
                        nameof(TreeNode.NodeAliasPath),
                        nameof(TreeNode.DocumentCulture),
                        nameof(TreeNode.DocumentName),
                        nameof(TreeNode.NodeLevel),
                        nameof(TreeNode.NodeSiteID)
                        })
                    .WithPageUrlPaths(); ;
    }

    public static DocumentQuery InRelationshipWithMany(this DocumentQuery baseQuery, IEnumerable<int> nodeIDs, string relationshipName) => nodeIDs.Any() ? baseQuery.Where(GetManyRelationshipsWhereInternal(nodeIDs, relationshipName)) : baseQuery;
    public static DocumentQuery<TDocument> InRelationshipWithMany<TDocument>(this DocumentQuery<TDocument> baseQuery, IEnumerable<int> nodeIDs, string relationshipName) where TDocument : TreeNode, new() => nodeIDs.Any() ? baseQuery.Where(GetManyRelationshipsWhereInternal(nodeIDs, relationshipName)) : baseQuery;
    public static MultiDocumentQuery InRelationshipWithMany<TDocument>(this MultiDocumentQuery baseQuery, IEnumerable<int> nodeIDs, string relationshipName) => nodeIDs.Any() ? baseQuery.Where(GetManyRelationshipsWhereInternal(nodeIDs, relationshipName)) : baseQuery;

    private static string GetManyRelationshipsWhereInternal(IEnumerable<int> nodeIDs, string relationshipName)
    {
        return $"NodeID in (select {nameof(RelationshipInfo.RightNodeId)} from {RelationshipInfo.TYPEINFO.GetTableName()} R inner join {RelationshipNameInfo.TYPEINFO.GetTableName()} RN on R.{nameof(RelationshipInfo.RelationshipNameId)} = RN.{nameof(RelationshipNameInfo.RelationshipNameId)} where {nameof(RelationshipNameInfo.RelationshipName)} = '{SqlHelper.EscapeQuotes(relationshipName)}' and {nameof(RelationshipInfo.LeftNodeId)} in ({string.Join(",", nodeIDs)}))";
    }
}