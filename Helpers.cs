using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using CBZ_To_Telegraph.ChapterParserAndExtractor;
using CBZ_To_Telegraph.Settings;
using CBZ_To_Telegraph.TelegramBot;
using CBZ_To_Telegraph.UserBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CBZ_To_Telegraph
{
    public static class Helpers
    {
        public static IServiceProvider BuildServiceProvider(IConfiguration configuration)
        {
   
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<UserBotConfig>();
            services.AddSingleton<UserBotWrapper>();
            
            services.AddSingleton<TelegramBotWrapperSettings>(
                (provider) => configuration.GetRequiredSection("TelegramBotSettings").Get<TelegramBotWrapperSettings>() ?? throw new InvalidOperationException());
            services.AddSingleton<TelegramBotWrapper>();
            
            services.AddSingleton<ArticleUploaderSettings>(provider =>
                    configuration.GetRequiredSection("ArticleUploaderSettings").Get<ArticleUploaderSettings>() ?? throw new InvalidOperationException());
            services.AddSingleton<ArticleUploader>();
            
            services.AddSingleton<MessageWithFileHandler>();
            services.AddSingleton<ImageConverter>();
            
           
            services.AddSingleton<ChapterParser>();
            services.AddSingleton<MessageFormatter>();
            return services.BuildServiceProvider();
        }
        
        
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var memStream = new MemoryStream())
            {
                using (var w = XmlWriter.Create(memStream))
                {
                    xmlDocument.WriteContentTo(w);
                }
                memStream.Seek(0, SeekOrigin.Begin);
                using (var r = XmlReader.Create(memStream))
                {
                    return XDocument.Load(r);
                }
            }
        }

        public static string GetMediaContentType(string pathtofile)
        {
            var filetype = pathtofile.Split(".").Last() switch
            {
                "jpg" => "image/jpeg",
                "png" => "image/png",
                "webp" => "image/webp",
                _ => throw new ArgumentException("Not Valid File Type")
            };
            return filetype;
        }
    }
}