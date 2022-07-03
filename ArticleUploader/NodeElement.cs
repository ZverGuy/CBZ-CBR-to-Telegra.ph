using System.Text.Json.Serialization;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    public class NodeElement
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; } = "img";
        [JsonPropertyName("attrs")]
        public NodeAtributes Attributes { get; set; }

        public NodeElement(string imagesrc)
        {
            Attributes = new NodeAtributes() {src = imagesrc};
        }
        public NodeElement() {}

        public class NodeAtributes
        {
            public string src { get; set; }
        }
    }
}