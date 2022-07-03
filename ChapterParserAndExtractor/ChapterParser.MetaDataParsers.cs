using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    public partial class ChapterParser
    {
        private enum Genre : int
        {
            ACTION = 0,
            ADULT = 1,
            ADVENTURE = 2,
            COMEDY = 3,
            DOUJINSHI = 4,
            DRAMA = 5,
            ECCHI = 6,
            FANTASY = 7,
            GENDERBENDER = 8,
            HAREM = 9,
            HISTORICAL = 10,
            HORROR = 11,
            JOSEI = 12,
            MAGIC = 13,
            MARTIALARTS = 14,
            MECHA = 15,
            MYSTERY = 16,
            ONESHOT = 17,
            PSYCHOLOGICAL = 18,
            ROMANCE = 19,
            SCHOOLLIFE = 20,
            SCIFI = 21,
            SEINEN = 22,
            SHOUJO = 23,
            SHOUJOAI = 24,
            SHOUNEN = 25,
            SHOUNENAI = 26,
            SLICEOFLIFE = 27,
            SPORTS = 28,
            SUPERNATURAL = 29,
            TRAGEDY = 30,
            YAOI = 31,
            YURI = 32
        }

        public MangaInfo GetMangaInfoFromComicXml(string unpackedDirectory)
        {
            string xmlstring = File.ReadAllText(unpackedDirectory + "\\ComicInfo.xml");
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlstring);
            XDocument xdoc = xmldoc.ToXDocument();
            MangaInfo result = new();
            result.Title = xdoc.Root.Descendants().FirstOrDefault(x => x.Name.LocalName == "Title").Value;
            result.Description = xdoc.Root.Descendants().FirstOrDefault(x => x.Name.LocalName == "Summary")?.Value;
            result.Genres = xdoc.Root.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "Genre")?
                .Value.Replace(" ", String.Empty)
                .Split(",").ToList();
            return result;
        }

        public MangaInfo GetMangaInfoFromBookJson(string unpackedDirectory)
        {
            string jsonstring = File.ReadAllText(unpackedDirectory + "\\book_info.json");
            JsonObject json = JsonNode.Parse(jsonstring).AsObject();
            MangaInfo result = new MangaInfo();
            result.Title = json["alt_title"] != null ? json["alt_title"].ToString() : json["title"].ToString();
            result.Description = json["description"].ToString();
            result.Tags = json["tags"]?.AsArray().Deserialize<List<string>>();
            result.Genres = GetGenresFromBookInfoJson(json["genres"]);
            return result;
        }

        public List<ChapterInfo> GetChapters(string unpackedDirectory)
        {
            List<ChapterInfo> result = new();
            foreach (var chapterdir in Directory.EnumerateDirectories(unpackedDirectory))
            {
                ChapterInfo chapter = new();
                chapter.Name = chapterdir.Split("\\").Last();
                foreach (var scanpath in Directory.EnumerateFiles(chapterdir))
                {
                    if (scanpath.Contains(".png") || scanpath.Contains(".jpg") || scanpath.Contains(".webp"))
                    {
                        chapter.ScanPaths.Add(scanpath);
                    }
                }
                result.Add(chapter);
            }

            return result;
        }

        public List<string> GetGenresFromBookInfoJson(JsonNode value)
        {
            try
            {
                var genresString = value.AsValue().ToString();
                return genresString.Split(",").ToList();
            }
            catch (Exception e)
            {
                var genresId = value.AsArray();
                return genresId.Select(x =>
                {
                    Genre genreenum = (Genre) Enum.Parse<Genre>(x.ToString());
                    
                    string type = genreenum switch
                    {
                        Genre.ACTION => "Экшен",
                        Genre.ADULT => "Для Взрослых",
                        Genre.YAOI => "Яой",
                        Genre.YURI => "Юри",
                        Genre.DRAMA => "Драма",
                        Genre.ECCHI => "Эччи",
                        Genre.HAREM => "Гарем",
                        Genre.JOSEI => "Дзёсэй",
                        Genre.MAGIC => "Магия",
                        Genre.MECHA => "Меха",
                        Genre.SCIFI => "Научная Фантастика",
                        Genre.COMEDY => "Комедия",
                        Genre.HORROR => "Хоррор",
                        Genre.SEINEN => "Сейнен",
                        Genre.SHOUJO => "Сёдзё",
                        Genre.SPORTS => "Спортивное",
                        Genre.FANTASY => "Фэнтеси",
                        Genre.MYSTERY => "Мистика",
                        Genre.ONESHOT => "Ваншот",
                        Genre.ROMANCE => "Романтика",
                        Genre.SHOUNEN => "Сёнен",
                        Genre.TRAGEDY => "Трагедия",
                        Genre.SHOUJOAI => "Юри",
                        Genre.HISTORICAL => "Историческое",
                        Genre.SCHOOLLIFE => "Школьная жизнь",
                        Genre.MARTIALARTS => "Боевые искусства",
                        Genre.SLICEOFLIFE => "повседневность",
                        Genre.GENDERBENDER => "Гендер-Бендер",
                        Genre.SUPERNATURAL => "Сверхъестественное",
                        Genre.PSYCHOLOGICAL => "Психологическое",
                        _ => "Неизвестный Жанр"

                    };
                    return type;
                }).ToList();
            }
        }
    }
}