using CardBot2.Handlers;
using CardBot2.Services;
using Telegram.Bot;

namespace CardBot2;

internal class Program
{
    static async Task Main()
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("TELEGRAM_TOKEN not set");
            return;
        }

        var botClient = new TelegramBotClient(token);
        var cardService = new CardService();
        var botHandler = new BotHandler(botClient, cardService);

        Console.WriteLine("Бот работает. Нажмите Ctrl+C что бы закрыть.");
        await botHandler.HandleUpdatesAsync();
    }
}