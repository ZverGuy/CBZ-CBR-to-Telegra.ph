using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CBZ_To_Telegraph.TelegramBot;
using TL;
using WTelegram;
using Message = Telegram.Bot.Types.Message;

namespace CBZ_To_Telegraph.UserBot
{
    public class UserBotWrapper
    {
        private readonly UserBotConfig _settings;
        private readonly Client _client;
        private readonly User _user;
        private readonly string _botUserName;

        public UserBotWrapper(UserBotConfig settings, TelegramBotWrapperSettings botsettings)
        {
            _settings = settings;
            _botUserName = botsettings.BotUserName;
            _client = new Client(_settings.GetConfig);
            _user = _client.LoginUserIfNeeded().Result;
        }


        public async Task DownloadFileToStreamAsync(Stream resultStream, int messageid)
        {
            try
            {
                var botapi = await _client.Contacts_ResolveUsername(_botUserName);
                var messagebase = await _client.Messages_GetHistory(botapi, limit:2);
                Document downloaddoc = null;
                foreach (var x1 in messagebase.Messages)
                {
                    
                    if (x1 is TL.Message message)
                    {
                        if (message.media is MessageMediaDocument mediadocument)
                        {
                            if (mediadocument.document is Document document)
                            {
                                downloaddoc = document;
                                break;
                            } 
                        }
                    }
                };
                var str = await _client.DownloadFileAsync(downloaddoc, resultStream, null,
                    (transmitted, size) => { Console.WriteLine($"Transmitted: {transmitted}; Size: {size}"); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

      
    }
}