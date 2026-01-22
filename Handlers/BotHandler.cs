using CardBot2.Constants;
using CardBot2.Context;
using CardBot2.Domain;
using CardBot2.Services;
using CardBot2.UI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace CardBot2.Handlers;

/// <summary>
/// Центральный обработчик входящих сообщений Telegram-бота.
///
/// НЕ управляет polling.
/// Получает уже готовое Message и:
/// - определяет команду
/// - работает с состоянием пользователя
/// - отправляет ответ
/// </summary>
public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICardService _cardService;
    private readonly IUserStateService _userStateService;

    public BotHandler(
        ITelegramBotClient botClient,
        ICardService cardService,
        IUserStateService userStateService)
    {
        _botClient = botClient;
        _cardService = cardService;
        _userStateService = userStateService;
    }

    /// <summary>
    /// Точка входа обработки одного сообщения.
    /// </summary>
    public async Task HandleAsync(Message message)
    {
        if (message.Text == null)
            return;

        var context = new BotContext(message);
        var currentState = _userStateService.GetState(context.ChatId);

        switch (context.MessageText)
        {
            case BotCommands.Start:
                _userStateService.SetState(context.ChatId, UserState.MainMenu);
                await SendStartMessage(context);
                break;

            case BotCommands.DrawCard:
                _userStateService.SetState(context.ChatId, UserState.ViewingCard);
                await SendRandomCard(context);
                break;

            default:
                // Неизвестные сообщения игнорируем
                break;
        }
    }

    private async Task SendStartMessage(BotContext context)
    {
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.StartMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }

    private async Task SendRandomCard(BotContext context)
    {
        var card = _cardService.GetRandomCard();

        if (!System.IO.File.Exists(card.ImagePath))
        {
            await _botClient.SendTextMessageAsync(
                chatId: context.ChatId,
                text: BotMessages.CardNotFound
            );
            return;
        }

        await using var stream = System.IO.File.OpenRead(card.ImagePath);

        await _botClient.SendPhotoAsync(
            chatId: context.ChatId,
            photo: new InputOnlineFile(
                stream,
                System.IO.Path.GetFileName(card.ImagePath)
            ),
            caption: $"{card.Name}\n\n{card.Description}"
        );
    }
}
