using CardBot2.Handlers;
using CardBot2.Handlers.Commands;
using CardBot2.Services;
using Telegram.Bot;

// Точка входа в приложение
// Здесь мы:
// 1. Создаём TelegramBotClient
// 2. Инициализируем сервисы
// 3. Регистрируем обработчики команд
// 4. Запускаем основной цикл бота

var botToken = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

if (string.IsNullOrWhiteSpace(botToken))
{
    throw new InvalidOperationException("Переменная окружения TELEGRAM_BOT_TOKEN не задана.");
}

// Клиент Telegram API
var botClient = new TelegramBotClient(botToken);

// Сервисы бизнес-логики
// Отвечают за работу с картами (без привязки к Telegram)
ICardService cardService = new CardService();

// Обработчики команд бота
// Каждый handler отвечает за одну конкретную команду
var commandHandlers = new List<ICommandHandler>
{
    new StartCommandHandler(botClient),
    new DrawCardCommandHandler(botClient, cardService)
};

// Центральный обработчик обновлений Telegram
// Он только маршрутизирует сообщения в нужные command handlers
var botHandler = new BotHandler(botClient, commandHandlers);

// Запуск основного цикла получения обновлений
await botHandler.HandleUpdatesAsync();