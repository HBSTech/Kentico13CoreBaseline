using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Generic.Models
{
    public class SearchResponse
    {
        public IEnumerable<SearchItem> Items { get; set; }
        public int TotalPossible { get; set; }
        public IEnumerable<string> HighlightedWords { get; set; }
        public Regex HighlightRegex { get; set; }
    }
}
