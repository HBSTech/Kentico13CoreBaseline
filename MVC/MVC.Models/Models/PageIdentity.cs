using System;

namespace Generic.Models
{
    public class PageIdentity
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public int NodeID { get; set; }
        public Guid NodeGUID { get; set; }
        public int DocumentID { get; set; }
        public Guid DocumentGUID { get; set; }
        public string Path { get; set; }
        public string RelativeUrl { get; set; }
        public string AbsoluteUrl { get; set; }
        public int NodeLevel { get; set; }
    }
}
