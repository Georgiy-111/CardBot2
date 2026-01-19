using CardBot2.Handlers;
using CardBot2.Services;
using Telegram.Bot;

namespace CardBot2;

/// <summary>
/// Точка входа в приложение.
/// 
/// Назначение Program.cs:
/// - получить токен Telegram бота из переменной окружения
/// - создать необходимые зависимости (BotClient, сервисы)
/// - запустить основной цикл обработки обновлений
/// 
/// ВАЖНО:
/// Вся бизнес-логика вынесена из этого файла.
/// Program.cs не содержит логики работы бота.
/// </summary>
internal class Program
{
    static async Task Main()
    {
        // Получаем токен из переменной окружения
        // Это безопаснее, чем хранить токен в коде
        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("TELEGRAM_TOKEN not set");
            return;
        }

        // Клиент Telegram API
        var botClient = new TelegramBotClient(token);

        // Сервис, отвечающий за работу с картами
        var cardService = new CardService();

        // Основной обработчик логики бота
        var botHandler = new BotHandler(botClient, cardService);

        Console.WriteLine("Бот работает. Нажмите Ctrl+C чтобы закрыть.");

        // Запуск бесконечного цикла получения обновлений
        await botHandler.HandleUpdatesAsync();
    }
}