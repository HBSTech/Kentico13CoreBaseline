using System.Text.RegularExpressions;

namespace Search.Models
{
    public class SearchResponse
    {
        public IEnumerable<SearchItem> Items { get; set; } = Array.Empty<SearchItem>();
        public int TotalPossible { get; set; } = 0;
        public IEnumerable<string> HighlightedWords { get; set; } = Array.Empty<string>();
        public Maybe<Regex> HighlightRegex { get; set; }
    }
}
