using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CBZ_To_Telegraph.TelegramBot
{
    public partial class TelegramBotWrapper
    {
       
            public static Task PollingErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(errorMessage);
                return Task.CompletedTask;
            }

            private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                try
                {
                    switch (update.Type)
                    {
                        case UpdateType.Message:
                        case UpdateType.EditedMessage:
                            OnMessageUpdate(botClient,
                                new OnMessageUpdateEventArgs() {Client = _client, Message = update.Message!});
                            break;
                        default:
                            await UnknownUpdateHandlerAsync(botClient, update);
                            break;
                    }
                }
#pragma warning disable CA1031
                catch (Exception exception)
#pragma warning restore CA1031
                {
                    await PollingErrorHandler(botClient, exception, cancellationToken);
                }

               

               
            }

            private Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update) { return Task.CompletedTask;}
    }
}