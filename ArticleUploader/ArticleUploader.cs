using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using CBZ_To_Telegraph.Settings;
using TL;
using JsonObject = System.Text.Json.Nodes.JsonObject;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    public class ArticleUploader
    {
        private readonly ArticleUploaderSettings _settings;
        private readonly ImageConverter _converter;
        private readonly string[] _proxys;

        public ArticleUploader(ArticleUploaderSettings settings, ImageConverter? converter)
        {
            _settings = settings;
            _converter = converter;
            _proxys = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + settings.ProxyTxtFileName).ToArray();
        }

        public async Task<List<ArticlePage>> UploadArticlesAsync(List<ChapterInfo> chapters, int maxparallel = default)
        {
            var result = new ConcurrentBag<KeyValuePair<double, ArticlePage>>();
            var chapterWithIndexers = new Dictionary<int, ChapterInfo>(chapters.Count());
            for (int i = 0; i < chapters.Count(); i++)
            {
                chapterWithIndexers.Add(i, chapters[i]);
            }
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = maxparallel != default ? maxparallel : _settings.MaxParallelThreadsForArticles
            };
            
            await Parallel.ForEachAsync(chapterWithIndexers, options, async (pair, token) =>
            {
               
                try
                {
                    var scans = await UploadScansAsync(pair.Value.ScanPaths);
                    //Superior Bad Request Fix
                    if (scans.Count > _settings.MaxScansPerChapter)
                    {
                        int counter = _settings.MaxScansPerChapter;
                        var spliscans = scans
                            .GroupBy(_ => counter++ / _settings.MaxScansPerChapter)
                            .Select(v => v.ToArray()).ToArray();
                        for (var index = 0; index < spliscans.Length; index++)
                        {
                            var split = spliscans[index];
                           
                            ArticlePage page = await UploadArticleAsync(pair.Value.Name + $". Часть {index + 1}", split);
                            result.Add(new KeyValuePair<double, ArticlePage>(pair.Key + index, page));
                        }
                    }
                    else
                    {
                        ArticlePage articlePage = await UploadArticleAsync(pair.Value.Name, scans);
                        result.Add(new KeyValuePair<double, ArticlePage>(pair.Key, articlePage));
                    }
                    
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    result.Add(new KeyValuePair<double, ArticlePage>(pair.Key, new ArticlePage()
                    {
                        title = pair.Value.Name + " [Ошибка при отправке]"
                    }));
                }
            });
            var articlePages = result
                .OrderBy(x => x.Key)
                .Select(x => x.Value)
                .ToList();
            return articlePages;
        }
        public async Task<List<string>> UploadScansAsync(List<string> scanspaths, int maxparralel = default)
        {
            var result = new ConcurrentBag<KeyValuePair<int, string>>();
            var scanPathsWithIndexers = new Dictionary<int, string>(scanspaths.Count());
            for (int i = 0; i < scanspaths.Count(); i++)
            {
                scanPathsWithIndexers.Add(i, scanspaths[i]);
            }

            var random = new Random();
           
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = maxparralel != default ? maxparralel : _settings.MaxParallelThreadsForScans
            };

            await Parallel.ForEachAsync(scanPathsWithIndexers, options, async (s, token) =>
            {
                using (var multiPartContent = new MultipartFormDataContent())
                {
                    HttpClient client;
                    //BAD CODE WARNING
                    if (_settings.EnableProxy)
                    {
                        try
                        {
                            client = new HttpClient(new HttpClientHandler()
                            {
                                Proxy = new WebProxy(_proxys[new Random().Next(_proxys.Length)])
                            });
                        }
                        catch (Exception e)
                        {
                            client = new HttpClient();
                        }
                       
                    }
                    else
                    {
                        client = new HttpClient();
                    }

                    string filepath = null;

                    if (s.Value.Split(".").Last() == "webp")
                    {
                        filepath = _converter.ConvertImageToJpg(s.Value);
                    }
                    else
                    {
                        filepath = s.Value;
                    }

                    using (FileStream fileStream = File.OpenRead(filepath))
                    {
                        var fileStreamContent = new StreamContent(fileStream);
                        var mediaContentType = Helpers.GetMediaContentType(filepath);
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(mediaContentType);

                        //Add the file
                        multiPartContent.Add(fileStreamContent, name: $"file", fileName: $"blob");
                        
                        await Task.Delay(TimeSpan.FromSeconds(_settings.UploadDelay));
                        Debug.WriteLine("UploadScansDelay");
                        var httpresponce = await client.PostAsync("https://telegra.ph/upload", multiPartContent);
                        string jsonstr = httpresponce.Content.ReadAsStringAsync().Result;
                        Debug.WriteLine(jsonstr);

                        var src = JsonObject.Parse(jsonstr).AsArray();
                        var str = src[0].AsObject()["src"].ToString();
                        result.Add(new KeyValuePair<int, string>(s.Key, str));
                    }
                }

             
            });
            return result
                .OrderBy(x => x.Key)
                .Select(x => x.Value)
                .ToList();
        }

        public async Task<ArticlePage> UploadArticleAsync(string title, IEnumerable<string> scans)
        {
            IEnumerable<NodeElement> elements = scans.Select(x => new NodeElement(x));
            string elementsstring = JsonSerializer.Serialize(elements);
            HttpClient client = new HttpClient();
            
            Debug.WriteLine("Content Bytes:" + System.Text.Encoding.Default.GetByteCount(elementsstring));
            
            await Task.Delay(TimeSpan.FromSeconds(_settings.UploadDelay));
            Debug.WriteLine("ArticleAsyncDelay");
            var responce = await client.GetAsync(
                $"https://api.telegra.ph/createPage?access_token={_settings.AccessToken}&title={title}&content={elementsstring}&return_content=true");
            var data = await responce.Content.ReadAsStringAsync();
            Debug.WriteLine("ArticleData: " +data);
            ArticlePage result = JsonNode.Parse(data).AsObject()["result"].Deserialize<ArticlePage>();
            return result;
        }

        public async Task<ArticlePage> UploadArticleAsync(MangaInfo info, IEnumerable<string> scansurls) =>
            await UploadArticleAsync(info.Title, scansurls);
    }
}