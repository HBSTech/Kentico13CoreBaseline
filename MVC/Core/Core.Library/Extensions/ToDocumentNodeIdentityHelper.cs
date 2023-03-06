namespace Core
{
    public static class ToDocumentNodeIdentityHelper
    {
        public static DocumentIdentity ToDocumentIdentity(this int value)
        {
            return new DocumentIdentity()
            {
                DocumentId = value
            };
        }
        public static DocumentIdentity ToDocumentIdentity(this string value, string? culture = null, int? siteId = null)
        {
            return new DocumentIdentity()
            {
                NodeAliasPathAndMaybeCultureAndSiteId = new Tuple<string, Maybe<string>, Maybe<int>>(value, (culture ?? string.Empty).AsNullOrWhitespaceMaybe(), (siteId ?? 0).WithMatchAsNone(0))
            };
        }
        public static DocumentIdentity ToDocumentIdentity(this Guid value)
        {
            return new DocumentIdentity()
            {
                DocumentGuid = value
            };
        }

        public static NodeIdentity ToNodeIdentity(this int value)
        {
            return new NodeIdentity()
            {
                 NodeId = value
            };
        }
        public static NodeIdentity ToNodeIdentity(this string value, int? siteId = null)
        {
            return new NodeIdentity()
            {
                NodeAliasPathAndSiteId = new Tuple<string, Maybe<int>>(value, (siteId ?? 0).WithMatchAsNone(0))
            };
        }
        public static NodeIdentity ToNodeIdentity(this Guid value)
        {
            return new NodeIdentity()
            {
                NodeGuid = value
            };
        }
    }
}
