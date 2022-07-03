using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace CBZ_To_Telegraph.TelegramBot
{
    public partial class TelegramBotWrapper
    {
        private readonly TelegramBotClient _client;

        public EventHandler<OnMessageUpdateEventArgs> OnMessageUpdate;
        private readonly TelegramBotWrapperSettings _settings;

        public TelegramBotWrapper(TelegramBotWrapperSettings settings)
        {
            _settings = settings;
            
            _client = new TelegramBotClient(new TelegramBotClientOptions(_settings.Token));
            WTelegram.Helpers.Log = ((i, s) => Debug.WriteLine($"{i}:{s}"));

        }
        public void RegisterMessageHandler(IMessageHandler handler)
        {
            OnMessageUpdate += async (sender, args) => await handler.HandleMessageAsync(sender, args);
        }

        public void Start()
        {
            _client.StartReceiving(updateHandler:  HandleUpdateAsync, pollingErrorHandler: PollingErrorHandler, null );
        }

    }
    
   

    
    public class OnMessageUpdateEventArgs
    {
        public ITelegramBotClient Client;
        public Message Message;
    }
}