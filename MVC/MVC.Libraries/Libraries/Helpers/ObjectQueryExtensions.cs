using CMS.DataEngine;
using CMS.DocumentEngine;

namespace Generic.Libraries.Helpers
{
    /// <summary>
    /// Extends the ObjectQuery to include a method that can take Columns but handle if it's null.
    /// </summary>
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

        
    }
}