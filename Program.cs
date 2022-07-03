// See https://aka.ms/new-console-template for more information

using System;
using CBZ_To_Telegraph;
using CBZ_To_Telegraph.TelegramBot;
using CBZ_To_Telegraph.UserBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;



Console.WriteLine("CBZ To Telegraph by kitsunoff");

IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
IServiceProvider provider = Helpers.BuildServiceProvider(config);

var bot = provider.GetService<TelegramBotWrapper>();


bot!.RegisterMessageHandler( provider.GetService<MessageWithFileHandler>()!);
bot!.RegisterMessageHandler(new GetChadIdHandler());
bot!.RegisterMessageHandler(new StartHandler());
bot.Start();

Console.ReadLine();









