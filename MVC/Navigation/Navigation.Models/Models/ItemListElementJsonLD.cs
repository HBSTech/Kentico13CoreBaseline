using System.Text.Json.Serialization;

namespace Navigation.Models
{
    public class ItemListElementJsonLD
    {
        public ItemListElementJsonLD(int position, string name, string item)
        {
            Position = position;
            Name = name;
            Item = item;
        }

        [JsonPropertyName("@type")]
        public string ContentType { get; set; } = "ListItem";
        [JsonPropertyName("position")]
        public int Position { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("item")]
        public string Item { get; set; }

     
    }
}