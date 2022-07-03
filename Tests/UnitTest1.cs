using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CBZ_To_Telegraph;
using CBZ_To_Telegraph.ChapterParserAndExtractor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Tests
{
    public class ChapterParserTests
    {
        private IServiceProvider _provider;
        private string updackedmangadir { get; set; }

        [SetUp]
        public void Setup()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _provider = Helpers.BuildServiceProvider(config);
            updackedmangadir = AppDomain.CurrentDomain.BaseDirectory + "test_manga\\unpacked";
        }


        [Test]
        public void TestBookJsonParser()
        {
            var MockMangaInfo = new MangaInfo()
            {
                Title = "Genjitsushugi Yuusha no Oukoku Saikenki",
                Description =
                    "Внезапно Сома Кадзуя оказывается в другом мире \u2014 его призвали сюда в качестве героя. Вот только его героическое путешествие... так и не началось. Услышав от короля об ужасном положении страны, он предлагает свой план по её возрождению, опираясь на знания из современного мира. Вскоре король отрекается от трона и передаёт корону Соме, да ещё и объявляет о его помолвке с принцессой!",
                Tags = new List<string>()
                {
                    "ГГ мужчина",
                    "Магия",
                    "Metaroka"
                }
            };
            ChapterParser parser = new ChapterParser();
            MangaInfo info = parser.GetMangaInfoFromBookJson(updackedmangadir);
            Assert.AreEqual(info.Title, MockMangaInfo.Title);
            Assert.AreEqual(info.Description, MockMangaInfo.Description);
            Assert.AreEqual(info.Tags, MockMangaInfo.Tags);

        }

        [Test]
        public void TestComicInfoXmlParser()
        {
            var MockMangaInfo = new MangaInfo()
            {
                Title = "Dance in the Vampire Bund",
                Description =
                    "Устав тысячелетиями скрываться от людского взора, Мина Цепеш, принцесса древнего договора и повелительница вампиров, жаждет перемен. С помощью богатого наследия рода Цепешей она полностью оплатила огромный национальный долг Японии, получив тем самым полномочия создать возле её берегов специальный округ, который должен стать пристанищем для вампиров со всего мира! И вот, в канун знаковой пресс-конференции, на которой человечество узнает о существовании вампиров, террористы и соперничающие группировки замышляют убить Мину до того, как она успеет сделать заявление, способное изменить мир...",
                Tags = new List<string>()
                {
                    "Seinen",
                    "Drama",
                    "Supernatural",
                    "Ecchi",
                    "Action",
                    "Schoollife",
                    "Romance"
                    
                }
            };
            ChapterParser parser = new ChapterParser();
            MangaInfo info = parser.GetMangaInfoFromComicXml(updackedmangadir);
            Assert.AreEqual(info.Title, MockMangaInfo.Title);
            Assert.AreEqual(info.Description, MockMangaInfo.Description);
            Assert.AreEqual(info.Tags, MockMangaInfo.Tags);
        }

        [Test]
        public async Task TestUploadScans()
        {
            ChapterParser parser = new ChapterParser();
            MangaInfo info = parser.GetFullMangaInformation(updackedmangadir);
            ArticleUploader uploader = _provider.GetService<ArticleUploader>();
            var result = await uploader.UploadScansAsync(info.Chapters[0].ScanPaths, 2);
            Assert.Pass();
        }


        [Test]
        public async Task TestCreatePage()
        {
            ChapterParser parser = new ChapterParser();
            MangaInfo info = parser.GetFullMangaInformation(updackedmangadir);
            ArticleUploader uploader = _provider.GetService<ArticleUploader>();
            var scans = await uploader.UploadScansAsync(info.Chapters[0].ScanPaths, 2);
            var result = await uploader.UploadArticleAsync("test", scans);
            Assert.NotNull(result);
        }

        [Test]
        public void TestImageConver()
        {
            var expectpath = AppDomain.CurrentDomain.BaseDirectory + "test_manga\\convert.png";
            ImageConverter converter = new ImageConverter();
            var path = converter.ConvertImageToJpg(AppDomain.CurrentDomain.BaseDirectory + "test_manga\\convert.webp");
            Assert.AreEqual(expectpath, path);
        }
    }
}

