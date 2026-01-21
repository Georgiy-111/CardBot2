using CardBot2.Constants;
using CardBot2.Context;
using CardBot2.Services;
using CardBot2.UI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace CardBot2.Handlers;

/// <summary>
/// Центральный обработчик сообщений Telegram-бота.
///
/// Класс НЕ управляет получением обновлений (polling).
/// Он получает уже готовое Message и:
/// - определяет, что за команда пришла
/// - вызывает нужную бизнес-логику
/// - отправляет ответ пользователю
///
/// Это делает код:
/// - понятным
/// - тестируемым
/// - расширяемым
/// </summary>
public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICardService _cardService;

    public BotHandler(
        ITelegramBotClient botClient,
        ICardService cardService)
    {
        _botClient = botClient;
        _cardService = cardService;
    }

    /// <summary>
    /// Основная точка входа для обработки входящего сообщения.
    /// Вызывается из TelegramUpdateLoop.
    /// </summary>
    public async Task HandleAsync(Message message)
    {
        // Защита от пустых сообщений (фото, стикеры и т.д.)
        if (message.Text == null)
            return;

        // Оборачиваем Message в контекст
        var context = new BotContext(message);

        // Определяем команду
        switch (context.MessageText)
        {
            case BotCommands.Start:
                await SendStartMessage(context);
                break;

            case BotCommands.DrawCard:
                await SendRandomCard(context);
                break;

            default:
                // Неизвестная команда — можно игнорировать
                break;
        }
    }

    /// <summary>
    /// Отправляет приветственное сообщение и клавиатуру.
    /// </summary>
    private async Task SendStartMessage(BotContext context)
    {
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.StartMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }

    /// <summary>
    /// Получает случайную карту и отправляет её пользователю.
    /// </summary>
    private async Task SendRandomCard(BotContext context)
    {
        var card = _cardService.GetRandomCard();

        // Проверяем, существует ли файл изображения на диске
        if (!System.IO.File.Exists(card.ImagePath))
        {
            await _botClient.SendTextMessageAsync(
                chatId: context.ChatId,
                text: BotMessages.CardNotFound
            );
            return;
        }

        // Открываем файл изображения как поток
        await using var stream = System.IO.File.OpenRead(card.ImagePath);

        // Отправляем изображение пользователю
        await _botClient.SendPhotoAsync(
            chatId: context.ChatId,
            photo: new InputOnlineFile(
                stream,
                Path.GetFileName(card.ImagePath)
            ),
            caption: $"{card.Name}\n\n{card.Description}"
        );
    }
}
