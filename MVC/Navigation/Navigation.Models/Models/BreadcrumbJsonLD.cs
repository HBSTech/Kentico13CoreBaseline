using System.Text.Json.Serialization;

namespace Navigation.Models
{
    public class BreadcrumbJsonLD
    {
        public BreadcrumbJsonLD(List<ItemListElementJsonLD> itemListElement)
        {
            ItemListElement = itemListElement;
        }

        [JsonPropertyName("@context")]
        public string Context { get; set; } = "https://schema.org";
        [JsonPropertyName("@type")]
        public string ContentType { get; set; } = "BreadcrumbList";
        [JsonPropertyName("itemListElement")]
        public List<ItemListElementJsonLD> ItemListElement { get; set; }

    
    }
}