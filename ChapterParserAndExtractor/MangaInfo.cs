using System.Collections.Generic;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    public class MangaInfo
    {
        public string? MainScanPath { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; } = new();
        public List<string>? Genres { get; set; } = new();
        public List<ChapterInfo> Chapters { get; set; } = new();
        
    }

    public class ChapterInfo
    {
        public string Name { get; set; }
        public List<string> ScanPaths { get; set; } = new();
    }
    
    
}