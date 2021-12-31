using Newtonsoft.Json;
using System;

namespace Generic.Models
{
    public class ItemListElementJsonLD
    {
        [JsonProperty(PropertyName = "@type")]
        public string ContentType { get; set; } = "ListItem";
        [JsonProperty(PropertyName = "position")]
        public int position { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "item")]
        public string Item { get; set; }

        public ItemListElementJsonLD()
        {
            
        }

        public ItemListElementJsonLD(Breadcrumb BreadcrumbItem, int Position)
        {
            
        }
    }
}