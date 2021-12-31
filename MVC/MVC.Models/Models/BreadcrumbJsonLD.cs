using Newtonsoft.Json;
using System.Collections.Generic;

namespace Generic.Models
{
    public class BreadcrumbJsonLD
    {
        [JsonProperty(PropertyName = "@context")]
        public string Context { get; set; } = "https://schema.org";
        [JsonProperty(PropertyName = "@type")]
        public string ContentType { get; set; } = "BreadcrumbList";
        [JsonProperty(PropertyName = "itemListElement")]
        public List<ItemListElementJsonLD> itemListElement { get; set; }

        public BreadcrumbJsonLD()
        {

        }
    }
}