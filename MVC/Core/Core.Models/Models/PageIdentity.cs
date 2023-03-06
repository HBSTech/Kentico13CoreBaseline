using MVCCaching;

namespace Core.Models
{
    public record PageIdentity : ICacheKey
    {
        public PageIdentity(string name, string alias, int nodeID, Guid nodeGUID, int documentID, Guid documentGUID, string path, string culture, string relativeUrl, string absoluteUrl, int nodeLevel, int nodeSiteID)
        {
            Name = name;
            Alias = alias;
            NodeID = nodeID;
            NodeGUID = nodeGUID;
            DocumentID = documentID;
            DocumentGUID = documentGUID;
            Path = path;
            RelativeUrl = relativeUrl;
            AbsoluteUrl = absoluteUrl;
            NodeLevel = nodeLevel;
            NodeSiteID = nodeSiteID;
            Culture = culture;
        }

        /// <summary>
        /// The Name of the page
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The Code Name of the page
        /// </summary>
        public string Alias { get; init; }

        /// <summary>
        /// The Page's int identity (culture agnostic)
        /// </summary>
        public int NodeID { get; init; }

        /// <summary>
        /// The Page's guid identity (culture agnostic)
        /// </summary>
        public Guid NodeGUID { get; init; }

        /// <summary>
        /// The Page's int identity (culture specific)
        /// </summary>
        public int DocumentID { get; init; }

        /// <summary>
        /// The Page's guid identity (culture specific)
        /// </summary>
        public Guid DocumentGUID { get; init; }

        /// <summary>
        /// The content path of the page
        /// </summary>
        public string Path { get; init; }

        /// <summary>
        /// Relative URL of the page, no tilde at beginning
        /// </summary>
        public string RelativeUrl { get; init; }

        /// <summary>
        /// Absolute URL of the page
        /// </summary>
        public string AbsoluteUrl { get; init; }

        /// <summary>
        /// Nesting level of the page.
        /// </summary>
        public int NodeLevel { get; init; }

        /// <summary>
        /// The Site ID of the page
        /// </summary>
        public int NodeSiteID { get; init; }

        public string Culture { get; set; }

        public DocumentIdentity DocumentIdentity
        {
            get
            {
                return new DocumentIdentity()
                {
                    DocumentId = DocumentID,
                    DocumentGuid = DocumentGUID,
                    NodeAliasPathAndMaybeCultureAndSiteId = new Tuple<string, Maybe<string>, Maybe<int>>(Path, Culture, NodeSiteID)
                };
            }
        }

        public NodeIdentity NodeIdentity
        {
            get
            {
                return new NodeIdentity()
                {
                    NodeId = NodeID,
                    NodeGuid = NodeGUID,
                    NodeAliasPathAndSiteId = new Tuple<string, Maybe<int>>(Path, NodeSiteID)
                };
            }
        }

        public string GetCacheKey()
        {
            return $"doc-{DocumentGUID}";
        }

        /// <summary>
        /// Returns an empty Page Identity with empty Guids and 0 valued ids.
        /// </summary>
        /// <returns></returns>
        public static PageIdentity Empty()
        {
            return new PageIdentity("", "", 0, Guid.Empty, 0, Guid.Empty, "/", "en-US", "/", "/", 0, 0);
        }

        /// <summary>
        /// Returns a page Identity with random Document/Node Guids
        /// </summary>
        /// <returns></returns>
        public static PageIdentity Random()
        {
            return new PageIdentity("", "", 0, Guid.NewGuid(), 0, Guid.NewGuid(), "/", "en-US", "/", "/", 0, 0);
        }
    }

    public record PageIdentity<T> : PageIdentity
    {
        public PageIdentity(string name, string alias, int nodeID, Guid nodeGUID, int documentID, Guid documentGUID, string path, string culture, string relativeUrl, string absoluteUrl, int nodeLevel, int nodeSiteID, T data) : base(name, alias, nodeID, nodeGUID, documentID, documentGUID, path, culture, relativeUrl, absoluteUrl, nodeLevel, nodeSiteID)
        {
            Data = data;
        }
        public PageIdentity(T data, PageIdentity pageIdentity) : base(pageIdentity)
        {
            Data = data;
        }

        /// <summary>
        /// Typed page data
        /// </summary>
        public T Data { get; set; }
    }
}
