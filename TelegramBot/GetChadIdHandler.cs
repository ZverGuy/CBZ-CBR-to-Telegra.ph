using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CBZ_To_Telegraph.TelegramBot
{
    public class GetChadIdHandler: IMessageHandler
    {
        public async Task HandleMessageAsync(object sender, OnMessageUpdateEventArgs eventArgs)
        {
            if (eventArgs.Message.Text != null && eventArgs.Message.Text.Contains("/chatid"))
            {
               await eventArgs.Client.SendTextMessageAsync(new ChatId(eventArgs.Message.Chat.Id), eventArgs.Message.Chat.Id.ToString());
            }
        }
    }
}