using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBZ_To_Telegraph.ChapterParserAndExtractor;

namespace CBZ_To_Telegraph.TelegramBot
{
    public class MessageFormatter
    {




        public string ArticleUrlsToTextHtml(IList<ArticlePage> pages)
        {
            if (pages == null || pages.Count() == 0) return "";
            StringBuilder builder = new();
            for (var index = 0; index < pages.Count(); index++)
            {
                var chapter = pages[index];
                builder = builder.AppendLine($"<a href=\"{pages[index].url}\">{pages[index].title}</a>");
            }

            return builder.ToString();
        }


        public string MangaInfoToString(MangaInfo info)
        {
            string tagsstring = GetTags(info.Tags);
            string genresstring = GetGenres(info.Genres);
            string chaptersstring = GetChapters(info.Chapters);
            return $"{info.Title}\n{info.Description}\n{tagsstring}\n{genresstring}\n\n{chaptersstring}";
        }

        public string ArticleInfoToString(MangaInfo? info, List<ArticlePage> pages)
        {
            string tagsstring = GetTags(info.Tags);
            string genresstring = GetGenres(info.Genres);
            string chapters = ArticleUrlsToTextHtml(pages);
            return $"{info.Title}\n{new string(info.Description.Take(650).ToArray()) + "..."}\n{tagsstring}\n{genresstring}\n\n{chapters}";
        }
        

        private string GetChapters(List<ChapterInfo> infoChapters)
        {
            if (infoChapters == null || infoChapters.Count == 0) return "";
            StringBuilder builder = new();
            for (var index = 0; index < infoChapters.Count; index++)
            {
                var chapter = infoChapters[index];
                builder = builder.AppendLine($"{index}) {chapter.Name}");
                Console.WriteLine(builder.ToString());
            }

            string result = builder.ToString();
            return result;
        }

        private string GetTags(List<string> infoTags)
        {
            if (infoTags == null || infoTags.Count == 0) return "";
            StringBuilder builder = new();
            foreach (var tag in infoTags)
            {
               builder = builder.Append("#" + tag.Replace(" ", "_") + ", ");
            }

            return builder.ToString();
        }
        private string GetGenres(List<string> genresTags)
        {
            if (genresTags == null || genresTags.Count == 0) return "";
            StringBuilder builder = new();
            foreach (var tag in genresTags)
            {
                builder = builder.Append("#" + tag.Replace(" ", "_") + ", ");
            }

            return builder.ToString();
        }
    }
}