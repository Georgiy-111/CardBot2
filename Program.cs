using CardBot2.Handlers;
using CardBot2.Infrastructure;
using CardBot2.Services;
using Telegram.Bot;

namespace CardBot2;

/// <summary>
/// Точка входа приложения.
/// 
/// Отвечает за:
/// - инициализацию зависимостей
/// - запуск TelegramUpdateLoop
/// </summary>
internal class Program
{
    static async Task Main()
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException(
                "Переменная окружения TELEGRAM_TOKEN не задана."
            );

        var botClient = new TelegramBotClient(token);

        // Сервисы
        var cardService = new CardService();
        var userStateService = new UserStateService();

        // Обработчик сообщений
        var botHandler = new BotHandler(
            botClient,
            cardService,
            userStateService
        );

        // Цикл получения обновлений
        var updateLoop = new TelegramUpdateLoop(
            botClient,
            botHandler
        );

        Console.WriteLine("Бот запущен.");
        await updateLoop.RunAsync();
    }
}