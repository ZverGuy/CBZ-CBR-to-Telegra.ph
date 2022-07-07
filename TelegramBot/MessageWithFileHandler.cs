using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CBZ_To_Telegraph.ChapterParserAndExtractor;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace CBZ_To_Telegraph.TelegramBot
{
    public class MessageWithFileHandler : IMessageHandler
    {
        private readonly UserBot.UserBotWrapper _userClient;
        private readonly TelegramBotWrapperSettings _botSettings;
        private readonly ChapterParser _parser;
        private readonly ArticleUploader _uploader;
        private readonly MessageFormatter _formatter;

        public MessageWithFileHandler(UserBot.UserBotWrapper userClient, 
            TelegramBotWrapperSettings botSettings, 
            ChapterParser parser, 
            ArticleUploader uploader,
            MessageFormatter formatter)
        {
            _userClient = userClient;
            _botSettings = botSettings;
            _parser = parser;
            _uploader = uploader;
            _formatter = formatter;
        }
        public async Task HandleMessageAsync(object? sender, OnMessageUpdateEventArgs eventArgs)
        {
            var botClient = eventArgs.Client;
            var messageId = eventArgs.Message.MessageId;
            var chatId = eventArgs.Message.Chat.Id;
            try
            {
                if(eventArgs.Message.Document == null) return;
                if(!isValidMimeType(eventArgs.Message.Document.MimeType)) return;

                using (var stream = new System.IO.MemoryStream())
                {
                    await botClient.ForwardMessageAsync(
                        _botSettings.ForwardChatId, new ChatId(chatId), messageId);
                
                    var botmessage =  await botClient.SendTextMessageAsync(chatId, "Downloading...");
                    await _userClient.DownloadFileToStreamAsync(stream, botmessage.MessageId);
                
                    await botClient.EditMessageTextAsync(chatId, botmessage.MessageId, "Extracting...");
                    var file = FileHelpers.WriteFileFromStream(stream, chatId+ "_" + messageId);
                    file.Close();
                    var directory = FileHelpers.UnpackZipFileToDirectory(chatId+ "_" + messageId);
                
                
                    var manga = _parser.GetFullMangaInformation(directory.FullName);
                
                    await botClient.EditMessageTextAsync(chatId, botmessage.MessageId, "Отправка в телеграф...\n" 
                        +_formatter.MangaInfoToString(manga));
                    // Message progressmessage = await botClient.SendTextMessageAsync(chatId, $"Uploaded Articles: 0/{manga.Chapters.Count}");
                    List<ArticlePage> articles = await _uploader.UploadArticlesAsync(manga.Chapters);

                    string text = _formatter.ArticleInfoToString(manga,articles);
                
                    if (manga.MainScanPath != null)
                    {
                        var scanfile = File.OpenRead(manga.MainScanPath);
                        await botClient.SendPhotoAsync(chatId, new InputMedia(scanfile, "1"), text, ParseMode.Html);
                        scanfile.Close();
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId,
                            text, ParseMode.Html);
                    }
                    await botClient.DeleteMessageAsync(chatId, botmessage.MessageId);
                    directory.Delete(recursive: true);
                }
                     
               

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await botClient.SendTextMessageAsync(chatId, $"Error while work with this file: \n{e.Message}");
                await botClient.SendTextMessageAsync(chatId, $"StackTrace: \n{e.StackTrace}");
            }
           

        }

        private bool isValidMimeType(string? documentMimeType)
        {
            bool result = documentMimeType switch
            {
                "application/vnd.comicbook+zip" or "application/vnd.comicbook-rar" or "application/x-cbr" or "application/x-cbz"=> true,
                _ => false
            };
            return result;
        }
    }
}