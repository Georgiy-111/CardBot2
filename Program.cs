using CardBot2.Handlers;
using CardBot2.Infrastructure;
using CardBot2.Services;
using Telegram.Bot;

namespace CardBot2;

/// <summary>
/// Точка входа приложения Telegram-бота.
/// 
/// Задачи Program.cs:
/// - считывает токен из переменной окружения
/// - инициализирует все сервисы и обработчики команд
/// - запускает основной цикл получения обновлений (TelegramUpdateLoop)
/// </summary>
internal class Program
{
    static async Task Main()
    {
        // Получение токена из переменной окружения
        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Переменная окружения TELEGRAM_TOKEN не задана."
            );
        }

        // Создание клиента Telegram API
        var botClient = new TelegramBotClient(token);

        // Инициализация сервисов
        var cardService = new CardService();
        var userStateService = new UserStateService();

        // Создание обработчиков команд
        var commandHandlers = new Handlers.Commands.ICommandHandler[]
        {
            new Handlers.Commands.StartCommandHandler(
                botClient,
                userStateService
            ),
            new Handlers.Commands.DrawCardCommandHandler(
                botClient,
                cardService,
                userStateService
            ),
            new Handlers.Commands.HelpCommandHandler(
                botClient)
        };

        // Основной обработчик сообщений
        var botHandler = new BotHandler(
            botClient,
            commandHandlers,
            userStateService
        );

        // Запуск цикла получения обновлений от Telegram
        var updateLoop = new TelegramUpdateLoop(
            botClient,
            botHandler
        );

        Console.WriteLine("Бот запущен.");
        await updateLoop.RunAsync();
    }
}