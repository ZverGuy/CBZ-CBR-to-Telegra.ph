using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    public class ArticlePage
    {
        public string path { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        public string image_url { get; set; }
        public List<NodeElement> content { get; set; }
        public int views { get; set; }

    }
}