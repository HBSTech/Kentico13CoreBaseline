using System;

namespace Generic.Models
{
    public record PageIdentityBase
    {
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
        /// Relative URL of the page
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
    }

    public record PageIdentity : PageIdentityBase
    {
        /// <summary>
        /// Generic page data
        /// </summary>
        public object Data { get; set; }
    }

    public record PageIdentity<T> : PageIdentityBase
    {
        /// <summary>
        /// Typed page data
        /// </summary>
        public T Data { get; set; }

        public static implicit operator PageIdentity(PageIdentity<T> d)
        {
            return new PageIdentity()
            {
                Name = d.Name,
                Alias = d.Alias,
                NodeID = d.NodeID,
                NodeGUID = d.NodeGUID,
                DocumentID = d.DocumentID,
                DocumentGUID = d.DocumentGUID,
                Path = d.Path,
                RelativeUrl = d.RelativeUrl,
                AbsoluteUrl = d.AbsoluteUrl,
                NodeLevel = d.NodeLevel,
                Data = d.Data
            };
        }
    }
}
