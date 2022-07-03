using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CBZ_To_Telegraph.TelegramBot
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(object sender, OnMessageUpdateEventArgs eventArgs);
    }
}