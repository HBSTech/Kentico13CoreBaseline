using System;

namespace Generic.Models
{
    public class SearchItem
    {
        public string DocumentExtensions { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public string Title { get; set; }
        public string Index { get; set; }
        public float MaxScore { get; set; }
        public int Position { get; set; }
        public double Score { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public float AbsScore { get; set; }
        public bool IsPage { get; set; } = false;
        public string PageUrl { get; set; }
        // Can customize this to add other fields you'll need
        // and set up Library AutomapperMaps
    }
}
