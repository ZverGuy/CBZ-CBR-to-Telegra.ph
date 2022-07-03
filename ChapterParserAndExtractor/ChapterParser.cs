using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    public partial class ChapterParser
    {
        public ChapterParser() {}

        public MangaInfo GetFullMangaInformation(string unpackedDirectory)
        {
            try
            {
                string? filename = Directory
                    .EnumerateFiles(unpackedDirectory)
                    .FirstOrDefault(x => x.Contains("book_info.json") || x.Contains("comic.xml"))
                    .Split("\\").Last();

                MangaInfo result = filename switch
                {
                    "book_info.json" => GetMangaInfoFromBookJson(unpackedDirectory),
                    "ComicInfo.xml" => GetMangaInfoFromComicXml(unpackedDirectory),
                    _ => throw new FileNotFoundException("book_info.json or ComicInfo.xml not found")
                };

                result.MainScanPath = Directory
                    .EnumerateFiles(unpackedDirectory)
                    .FirstOrDefault(x => x.Contains(".png") || x.Contains(".jpg") || x.Contains(".webp"));
                result.Chapters = GetChapters(unpackedDirectory);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}