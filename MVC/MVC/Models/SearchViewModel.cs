using CMS.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            SearchItems = new List<SearchResultItem>();
        }
        public string SearchValue { get; set; }
        public List<SearchResultItem> SearchItems { get; set; }
        
    }
}