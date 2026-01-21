using CardBot2.Handlers;
using CardBot2.Infrastructure;
using CardBot2.Services;
using Telegram.Bot;

namespace CardBot2;

/// <summary>
/// Точка входа приложения.
/// 
/// Отвечает ТОЛЬКО за:
/// - чтение конфигурации
/// - создание зависимостей
/// - запуск Telegram-бота
/// </summary>
internal class Program
{
    static async Task Main()
    {
        // Читаем токен из переменной окружения
        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Переменная TELEGRAM_TOKEN не задана");
            return;
        }

        // Клиент Telegram API
        var botClient = new TelegramBotClient(token);

        // Бизнес-логика
        ICardService cardService = new CardService();

        // Обработчик входящих сообщений
        var botHandler = new BotHandler(botClient, cardService);

        // Цикл получения обновлений
        var updateLoop = new TelegramUpdateLoop(botClient, botHandler);

        Console.WriteLine("Бот запущен. Нажмите Ctrl+C для выхода.");

        // Запуск long polling
        await updateLoop.RunAsync();
    }
}