using System.Text.Json.Serialization;

namespace AdventureGame
{
    // items
    internal class ItemParser
    {
        [JsonPropertyName("name")]
        public string? Name { get;  set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("imageLocation")]
        public string? ImageLocation { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get;  set; }

        [JsonPropertyName("wearableType")]
        public string? WearableType { get; set; }

        [JsonPropertyName("sprite")]
        public char Sprite { get;  set; }

        [JsonPropertyName("value")]
        public long Value { get; set; }

        [JsonPropertyName("weight")]
        public int Weight { get;  set; }

        [JsonPropertyName("damage")]
        public int? Damage { get;  set; }

        [JsonPropertyName("attack")]
        public int? Attack { get; set; }

        [JsonPropertyName("defence")]
        public int? Defence { get; set; }

        [JsonPropertyName("speed")]
        public int? Speed { get; set; }

        [JsonPropertyName("sustenance")]
        public int? Sustenance { get; set; }

        [JsonPropertyName("flags")]
        public List<string> Flags { get; } = new List<string>();
    }

    // UI
    public class ElementParser
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("render")]
        public List<string> Render { get; set; }

        [JsonPropertyName("position")]
        public List<int> Position { get; set; }
    }

    public class AllElements
    {
        [JsonPropertyName("element")]
        public List<ElementParser> Element { get; set; }
    }


}
